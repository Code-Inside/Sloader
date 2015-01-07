using System;
using System.Threading.Tasks;
using Sloader.Crawler.DependencyServices;
using Sloader.Crawler.Feed;
using Sloader.Crawler.Twitter;
using Sloader.Types;

namespace Sloader.Crawler
{
    public class MasterCrawler
    {
        private readonly MasterCrawlerConfig _config;
        private readonly MasterCrawlerSecrets _secrets;

        public MasterCrawler(MasterCrawlerConfig config, MasterCrawlerSecrets secrets)
        {
            _config = config;
            _secrets = secrets;
        }

        public async Task<CrawlerRun> RunAllCrawlers()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var crawlerRunResult = new CrawlerRun();

            // Feeds
            if (string.IsNullOrWhiteSpace(_config.Feeds) == false)
            {
                foreach (var feed in _config.Feeds.Split(';'))
                {
                    var feedCrawler = new FeedCrawler();
                    feedCrawler.ConfiguredFeed = feed;
                    var feedResult = await feedCrawler.DoWorkAsync();
                    crawlerRunResult.Results.Add(feedResult);
                }
              
            }

            // Tweets
            if (_config.TwitterHandles != null &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerKey) == false &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerSecret) == false)
            {
                ITwitterOAuthTokenService oAuthTokenLoader = new TwitterOAuthTokenService();
                var oauth = await oAuthTokenLoader.GetAsync(_secrets.TwitterConsumerKey, _secrets.TwitterConsumerSecret);
                if (oauth != string.Empty)
                {
                    var twitterCrawler = new TwitterTimelineCrawler();
                    twitterCrawler.Config.Handles = _config.TwitterHandles;
                    twitterCrawler.Config.OAuthToken = oauth;

                    var twitterResults = await twitterCrawler.DoWorkAsync();
                    crawlerRunResult.Results.AddRange(twitterResults);
                }
                
            }

            watch.Stop();
            crawlerRunResult.RunDurationInMilliseconds = watch.ElapsedMilliseconds;
            crawlerRunResult.RunOn = DateTime.UtcNow;

            return crawlerRunResult;
        }
    }
}