using System.Collections.Generic;
using Sloader.Config.Crawler.Feed;
using Sloader.Config.Crawler.GitHub;
using Sloader.Config.Crawler.Twitter;

namespace Sloader.Config.Crawler
{
    /// <summary>
    /// Container for all Crawler configs. Each crawlerﬂtype can occure only time, but the target for the crawler is a list, but each list must have it's own unique key.
    /// </summary>
    /// <example>
    /// Demo yml config (only structure for Crawlers):
    /// <code>
    /// Crawler:
    ///   FeedsToCrawl:
    ///   - Key: Blog
    ///     Url: http://blog.codeinside.eu/feed
    ///     LoadSocialLinkCounters: false
    ///   - Key: BlogTwo
    ///     Url: http://blog.codeinside.eu/feed2
    ///     LoadSocialLinkCounters: false
    ///   - Key: BlogThreeCombined
    ///     Url: http://blog.codeinside.eu/feed3;http://blog.codeinside.eu/feed4
    ///     LoadSocialLinkCounters: false
    ///   OtherCrawler...
    /// </code>
    /// </example>
    public class CrawlerConfig
    {
        public CrawlerConfig()
        {
            FeedsToCrawl = new List<FeedCrawlerConfig>();
            TwitterTimelinesToCrawl = new List<TwitterTimelineCrawlerConfig>();
            TwitterUsersToCrawl = new List<TwitterUserCrawlerConfig>();
            GitHubEventsToCrawl = new List<GitHubEventCrawlerConfig>();
        }

        /// <summary>
        /// Represents all FeedCrawlers
        /// </summary>
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; }

        /// <summary>
        /// Represents all TwitterTimelinesToCrawls
        /// </summary>
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; }

        /// <summary>
        /// Represents all TwitterUsersToCrawls
        /// </summary>
        public IList<TwitterUserCrawlerConfig> TwitterUsersToCrawl { get; set; }

        /// <summary>
        /// Represents all GitHubEventCrawlers
        /// </summary>
        public IList<GitHubEventCrawlerConfig> GitHubEventsToCrawl { get; set; }

    }
}