using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Sloader.Config;
using System.Linq;
using System.Runtime.InteropServices;
using Sloader.Engine.Crawler.DependencyServices;
using Sloader.Engine.Crawler.Feed;
using Sloader.Engine.Crawler.Twitter;
using Sloader.Engine.Drop.File;
using Sloader.Engine.Drop.GitHub;
using Sloader.Result;

namespace Sloader.Engine
{
    public class SloaderRunner
    {
        public static async Task AutoRun()
        {
            if (ConfigurationManager.AppSettings[FixedConfigKeys.SloaderConfigPath] != null)
            {
                await AutoRunInternal(ConfigurationManager.AppSettings[FixedConfigKeys.SloaderConfigPath], ConfigurationManager.AppSettings.AllKeys.ToDictionary(k => k,
                                v => ConfigurationManager.AppSettings[v]));
            }
            else
            {
                Trace.TraceError($"AppSetting Key with path to Sloader.yml missing: '{FixedConfigKeys.SloaderConfigPath}'");
            }
        }

        public static async Task AutoRun(string sloaderConfigPath, Dictionary<string, string> secrets)
        {
            await AutoRunInternal(sloaderConfigPath, secrets);
        }

        private static async Task AutoRunInternal(string sloaderConfigPath, Dictionary<string, string> secrets)
        {
            var config =
                 await
                     SloaderConfig.Load(sloaderConfigPath, secrets);

            var runner = new SloaderRunner(config);
            var crawlerRun = await runner.RunAllCrawlers();
            await runner.RunThroughDrop(crawlerRun);
        }

        private readonly SloaderConfig _config;

        public SloaderRunner(SloaderConfig config)
        {
            _config = config;
        }

        public async Task<CrawlerRun> RunAllCrawlers()
        {
            var watch = new Stopwatch();
            watch.Start();

            var crawlerRunResult = new CrawlerRun();

            if (_config.Crawler == null)
                return crawlerRunResult;

            // Feeds
            if (_config.Crawler.FeedsToCrawl.Any())
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
                    if (_config.Crawler.TwitterTimelinesToCrawl.Any())
                    {
                        foreach (var twitterConfig in _config.Crawler.TwitterTimelinesToCrawl)
                        {
                            var twitterTimelineCrawler = new TwitterTimelineCrawler();
                            twitterTimelineCrawler.OAuthToken = oauth;

                            var twitterTimelineResult = await twitterTimelineCrawler.DoWorkAsync(twitterConfig);
                            crawlerRunResult.AddResultDataPair(twitterConfig.Key, twitterTimelineResult);
                        }
                    }

                    if (_config.Crawler.TwitterUsersToCrawl.Any())
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

        public async Task RunThroughDrop(CrawlerRun crawlerRun)
        {
            if (_config.Drop != null)
            {
                foreach (var fileDropConfig in _config.Drop.FileDrops)
                {
                    var fileDrop = new FileDrop();
                    await fileDrop.DoWorkAsync(fileDropConfig, crawlerRun);
                }

                foreach (var gitHubDropConfig in _config.Drop.GitHubDrops)
                {
                    if (!string.IsNullOrWhiteSpace(_config.Secrets.GitHubAccessToken))
                    {
                        var gitHubDrop = new GitHubDrop();
                        gitHubDrop.AccessToken = _config.Secrets.GitHubAccessToken;
                        await gitHubDrop.DoWorkAsync(gitHubDropConfig, crawlerRun);
                    }
                }
            }

        }
    }
}