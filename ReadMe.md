## Full Sample Sloader.yml

Secrets:
  TwitterConsumerKey: $$Sloader.SecretTwitterConsumerKey$$
  TwitterConsumerSecret: $$Sloader.SecretTwitterConsumerSecret$$
  GitHubAccessToken: $$Sloader.SecretGitHubAccessToken$$

Crawler:
  FeedsToCrawl:
  - Key: Blog
    Url: http://blogin.codeinside.eu/feed
    LoadSocialLinkCounters: false
  - Key: GitHub
    Url: https://github.com/robertmuehsig.atom; https://github.com/oliverguhr.atom
    LoadSocialLinkCounters: false
  TwitterTimelinesToCrawl:
  - Handle: codeinsideblog;robert0muehsig;oliverguhr
    IncludeRetweets: false
    Key: Twitter
  TwitterUsersToCrawl:
  - Handle: robert0muehsig
    Key: TwitterMe
  GitHubEventsToCrawl:
  - Key: GitHubEvent
    Repo: code-inside/sloader;foobar/test
	User: robertmuehsig;oliverguhr
	Orgs: code-inside
    
Drop:
  FileDrops:
  - FilePath: "test.json"
  - FilePath: "test2.json"
  GitHubDrops:
  - Owner: "Code-Inside"
    Repo: "Hub"
    Branch: "gh-pages"
    FilePath: "_data/test.json"