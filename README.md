This program monitors posts made to a specified Subreddit.  To run the program, you must first get a refresh token by using the following steps:
	1. Go to:  https://not-an-aardvark.github.io/reddit-oauth-helper
	2. Enter:
		a. Client ID:  RkYzlFkkHHdWBOP0ku5vDA
		b. Client Secret: obA6WMMq_txLJ39tU-UGpUFyKsQwkA
		c. Check Permanent
		d. Check Identity and Read
	3. Submit the page.  On the next page, click Allow.  The next page will give you the refresh token and the access token.
	4. Use the refresh token to run EMMonitor, e.g.:  EMMonitor -r <Refresh Token> -s <Subreddit>
