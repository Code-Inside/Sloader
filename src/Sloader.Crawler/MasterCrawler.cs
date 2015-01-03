using System;
using System.Threading.Tasks;
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
            if (_config.Feeds != null)
            {
                var feedCrawler = new FeedCrawler();
                feedCrawler.ConfiguredFeeds = _config.Feeds;
                var feedResults = await feedCrawler.DoWorkAsync();
                crawlerRunResult.Results.AddRange(feedResults);
            }

            // Tweets
            if (_config.TwitterHandles != null &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerKey) == false &&
                string.IsNullOrEmpty(_secrets.TwitterConsumerSecret) == false)
            {
                var twitterCrawler = new TwitterTimelineCrawler();
                twitterCrawler.Config.Handles = _config.TwitterHandles;
                twitterCrawler.Config.ConsumerKey = _secrets.TwitterConsumerKey;
                twitterCrawler.Config.ConsumerSecret = _secrets.TwitterConsumerSecret;

                var twitterResults = await twitterCrawler.DoWorkAsync();
                crawlerRunResult.Results.AddRange(twitterResults);
            }

            watch.Stop();
            crawlerRunResult.RunDurationInMilliseconds = watch.ElapsedMilliseconds;
            crawlerRunResult.RunOn = DateTime.UtcNow;

            return crawlerRunResult;
        }
    }
}