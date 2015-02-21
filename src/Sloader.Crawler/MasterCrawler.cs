using System;
using System.Linq;
using System.Threading.Tasks;
using Sloader.Crawler.Config;
using Sloader.Crawler.DependencyServices;
using Sloader.Crawler.Feed;
using Sloader.Crawler.Twitter;
using Sloader.Results;

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
            if (_config.FeedsToCrawl.Any())
            {
                foreach (var feed in _config.FeedsToCrawl)
                {
                    var feedCrawler = new FeedCrawler();
                    var feedResult = await feedCrawler.DoWorkAsync(feed);
                    crawlerRunResult.Results.Add(feedResult);
                }
              
            }

            // Tweets
            if (_config.TwitterTimelinesToCrawl.Any() && _secrets.IsTwitterConsumerConfigured)
            {
                ITwitterOAuthTokenService oAuthTokenLoader = new TwitterOAuthTokenService();
                var oauth = await oAuthTokenLoader.GetAsync(_secrets.TwitterConsumerKey, _secrets.TwitterConsumerSecret);
                if (string.IsNullOrWhiteSpace(oauth) == false)
                {
                    foreach (var handle in _config.TwitterTimelinesToCrawl)
                    {
                        var twitterCrawler = new TwitterTimelineCrawler();
                        twitterCrawler.OAuthToken = oauth;

                        var twitterResult = await twitterCrawler.DoWorkAsync(handle);
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