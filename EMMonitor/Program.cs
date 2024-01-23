using CommandLine;

using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace EMMonitor
{
    public class Options
    {
        [Option('r', "refreshtoken", Required = true, HelpText = "Enter your Reddit refresh token.")]
        public string RefreshToken { get; set; }

        [Option('s', "subreddit", Required = true, HelpText = "Enter your desired subreddit.")]
        public string Subreddit { get; set; }
    }

    class Program
    {
        static string AppId;
        static string AppSecret;

        static RedditClient _reddit;

        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            AppId = config["RedditOptions:AppId"];
            AppSecret = config["RedditOptions:AppSecret"];

            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);

            Console.WriteLine("Done!");
        }

        static void RunOptions(Options opts)
        {
            _reddit = new RedditClient(appId: AppId, appSecret: AppSecret, refreshToken: opts.RefreshToken);

            var subreddit = _reddit.Subreddit(opts.Subreddit);

            // Start monitoring subreddit for new posts.
            subreddit.Posts.GetNew();
            subreddit.Posts.NewUpdated += NewPostsUpdated;
            subreddit.Posts.MonitorNew();

            Console.WriteLine("Press spacebar to display statistics, ESC to quit.");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        var usersPosts = DataHelper.UsersPosts;
                        if ((usersPosts == null) || (usersPosts.Count == 0))
                        {
                            Console.WriteLine("No users data to report.");
                        }
                        else
                        {
                            Console.WriteLine("Users by Number of Posts:");

                            // Sort the users by the number of posts they created.
                            var dict = new Dictionary<string, int>();
                            foreach (string userId in usersPosts.Keys)
                            {
                                dict.Add(userId, usersPosts[userId].Count);
                            }
                            var ordered = dict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                            foreach (string userId in ordered.Keys)
                            {
                                Console.WriteLine($"User: {userId}, Num. Posts: {ordered[userId]}");
                            }
                        }

                        var posts = DataHelper.Posts;
                        if ((posts == null) || (posts.Count == 0))
                        {
                            Console.WriteLine("No posts data to report.");
                        }
                        else
                        {
                            Console.WriteLine("Posts by Upvotes:");

                            // Sort posts by the number of upvotes.
                            var sortedPosts = posts.OrderByDescending(x => x.Score).ToList();
                            foreach (var post in sortedPosts)
                            {
                                Console.WriteLine($"Post: {post.Title}, UpVotes: {post.Score}");
                            }

                        }
                    }
                    else if (key == ConsoleKey.Escape) { break; }
                }
            }

            // Stop monitoring subreddit for new posts.
            subreddit.Posts.MonitorNew();
            subreddit.Posts.NewUpdated -= NewPostsUpdated;
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            // Display help or error messages to the user
            foreach (var error in errors)
            {
                Console.WriteLine($"Error: {error}");
            }
        }

        #region Support Methods

        /// <summary>
        /// Whenever a new post is detected, it is added to the list of posts indexed by its Id.  The post's score is then monitored
        /// so that we're notified when it changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            foreach (var post in e.Added)
            {
#if DEBUG
                Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
#endif
                DataHelper.Add(new RedditPost { Id = post.Id, Title = post.Title, Author = post.Author, Score = post.Score });

                // Monitor the post for changes to its score.
                post.MonitorPostScore(minScoreMonitoringThreshold: 1);
                post.PostScoreUpdated += PostScoreUpdated;
            }
        }

        // Callback function that receives event when post score changes
        static void PostScoreUpdated(object sender, PostUpdateEventArgs e)
        {
#if DEBUG
            Console.WriteLine($"The monitored post score is now {e.NewPost.Score}.");
#endif
            DataHelper.Posts.Find(x => x.Id.Equals(e.NewPost.Id)).Score = e.NewPost.Score;
        }

        #endregion
    }
}
