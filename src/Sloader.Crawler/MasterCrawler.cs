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
            if (string.IsNullOrWhiteSpace(_config.TwitterHandles) == false &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerKey) == false &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerSecret) == false)
            {
                ITwitterOAuthTokenService oAuthTokenLoader = new TwitterOAuthTokenService();
                var oauth = await oAuthTokenLoader.GetAsync(_secrets.TwitterConsumerKey, _secrets.TwitterConsumerSecret);
                if (string.IsNullOrWhiteSpace(oauth) == false)
                {
                    foreach (var handle in _config.TwitterHandles.Split(';'))
                    {
                        var twitterCrawler = new TwitterTimelineCrawler();
                        twitterCrawler.Config.Handle = handle;
                        twitterCrawler.Config.OAuthToken = oauth;

                        var twitterResult = await twitterCrawler.DoWorkAsync();
                        crawlerRunResult.Results.Add(twitterResult);
                    }
                }
                
            }

            watch.Stop();
            crawlerRunResult.RunDurationInMilliseconds = watch.ElapsedMilliseconds;
            crawlerRunResult.RunOn = DateTime.UtcNow;

            return crawlerRunResult;
        }
    }
}