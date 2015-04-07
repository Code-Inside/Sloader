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
    public class SloaderRunner
    {
        private readonly SloaderConfig _config;
        private readonly SloaderSecrets _secrets;

        public SloaderRunner(SloaderConfig config, SloaderSecrets secrets)
        {
            _config = config;
            _secrets = secrets;
        }

        public async Task<CrawlerRun> RunAllCrawlers()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var crawlerRunResult = new CrawlerRun();

            if (_config.Crawler == null)
                return crawlerRunResult;

            // Feeds
            if (_config.Crawler.FeedsToCrawl.Any())
            {
                foreach (var feed in _config.Crawler.FeedsToCrawl)
                {
                    var feedCrawler = new FeedCrawler();
                    var feedResult = await feedCrawler.DoWorkAsync(feed);
                    crawlerRunResult.Results.Add(feedResult);
                }
              
            }

            // Tweets
            if (_config.Crawler.TwitterTimelinesToCrawl.Any() && _secrets.IsTwitterConsumerConfigured)
            {
                ITwitterOAuthTokenService oAuthTokenLoader = new TwitterOAuthTokenService();
                var oauth = await oAuthTokenLoader.GetAsync(_secrets.TwitterConsumerKey, _secrets.TwitterConsumerSecret);
                if (string.IsNullOrWhiteSpace(oauth) == false)
                {
                    foreach (var handle in _config.Crawler.TwitterTimelinesToCrawl)
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