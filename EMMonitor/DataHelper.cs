namespace EMMonitor
{
    /// <summary>
    /// The DataHelper class maintains a list of all posts that have been created since the monitoring API was first initialized.
    /// </summary>
    public static class DataHelper
    {
        public static List<RedditPost> Posts { get; private set; } = new List<RedditPost>();
        public static Dictionary<string, List<string>> UsersPosts { get; private set; } = new Dictionary<string, List<string>>();

        public static void Add(RedditPost post)
        {
            Posts.Add(post);

            if (UsersPosts.ContainsKey(post.Author))
            {
                UsersPosts[post.Author].Add(post.Title);
            }
            else
            {
                UsersPosts.Add(post.Author, new List<string> { post.Title });
            }
        }
    }
}
