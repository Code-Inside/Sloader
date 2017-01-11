using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sloader.Config;
using System.Linq;
using Sloader.Engine.Crawler.DependencyServices;
using Sloader.Engine.Crawler.Feed;
using Sloader.Engine.Crawler.Twitter;
using Sloader.Result;

namespace Sloader.Engine
{
    public class SloaderRunner
    {
        private readonly SloaderConfig _config;

        public SloaderRunner(SloaderConfig config)
        {
            _config = config;
        }

        public async Task<CrawlerRun> RunAllCrawlers()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var crawlerRunResult = new CrawlerRun();

            if (_config.Crawler == null)
                return crawlerRunResult;

            // Feeds
            if (_config.Crawler.FeedsToCrawl != null && _config.Crawler.FeedsToCrawl.Any())
            {
                foreach (var feedConfig in _config.Crawler.FeedsToCrawl)
                {
                    var feedCrawler = new FeedCrawler();
                    var feedResult = await feedCrawler.DoWorkAsync(feedConfig);
                    crawlerRunResult.AddResultDataPair(feedConfig.Key, feedResult);
                }

            }

            // Tweets
            if (_config.Secrets.IsTwitterConsumerConfigured)
            {
                ITwitterOAuthTokenService oAuthTokenLoader = new TwitterOAuthTokenService();
                var oauth = await oAuthTokenLoader.GetAsync(_config.Secrets.TwitterConsumerKey, _config.Secrets.TwitterConsumerSecret);

                if (string.IsNullOrWhiteSpace(oauth) == false)
                {
                    if (_config.Crawler.TwitterTimelinesToCrawl != null && _config.Crawler.TwitterTimelinesToCrawl.Any())
                    {
                        foreach (var twitterConfig in _config.Crawler.TwitterTimelinesToCrawl)
                        {
                            var twitterTimelineCrawler = new TwitterTimelineCrawler();
                            twitterTimelineCrawler.OAuthToken = oauth;

                            var twitterTimelineResult = await twitterTimelineCrawler.DoWorkAsync(twitterConfig);
                            crawlerRunResult.AddResultDataPair(twitterConfig.Key, twitterTimelineResult);
                        }
                    }

                    if (_config.Crawler.TwitterUsersToCrawl != null &&_config.Crawler.TwitterUsersToCrawl.Any())
                    {
                        foreach (var twitterConfig in _config.Crawler.TwitterUsersToCrawl)
                        {
                            var twitterUserCrawler = new TwitterUserCrawler();
                            twitterUserCrawler.OAuthToken = oauth;

                            var twitterUserResult = await twitterUserCrawler.DoWorkAsync(twitterConfig);
                            crawlerRunResult.AddResultDataPair(twitterConfig.Key, twitterUserResult);
                        }
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