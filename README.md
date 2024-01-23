This program monitors posts made to a specified Subreddit.  To run the program, you must first get a refresh token by using the following steps:
<ol>
	<li>Go to:  <a href="https://not-an-aardvark.github.io/reddit-oauth-helper">https://not-an-aardvark.github.io/reddit-oauth-helper</a></li>
	<li>Enter:</li>
	<ol>
		<li>Client ID:  RkYzlFkkHHdWBOP0ku5vDA</li>
		<li>Client Secret: obA6WMMq_txLJ39tU-UGpUFyKsQwkA</li>
		<li>Check Permanent</li>
		<li>Check Identity and Read</li>
	</ol>
	<li>Submit the page.  On the next page, click Allow.  The next page will give you the refresh token and the access token.</li>
	<li>Use the refresh token to run EMMonitor, e.g.:  EMMonitor -r &ltRefresh Token&gt -s &ltSubreddit&gt</li>
</ol>
