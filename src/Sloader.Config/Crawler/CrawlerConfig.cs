using System.Collections.Generic;
using Sloader.Config.Crawler.Feed;
using Sloader.Config.Crawler.GitHub;

namespace Sloader.Config.Crawler
{
    /// <summary>
    /// Container for all Crawler configs. 
    /// <para>Each crawler type can occure only time, but the target for the crawler is a list.</para>
    /// <para>Each list must have it's own unique key.</para> 
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
            GitHubEventsToCrawl = new List<GitHubEventCrawlerConfig>();
            GitHubIssuesToCrawl = new List<GitHubIssueCrawlerConfig>();
        }

        /// <summary>
        /// Represents all FeedCrawlers
        /// </summary>
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; }

        /// <summary>
        /// Represents all GitHubEventCrawlers
        /// </summary>
        public IList<GitHubEventCrawlerConfig> GitHubEventsToCrawl { get; set; }

        /// <summary>
        /// Represents all GitHubIssuesToCrawl
        /// </summary>
        public IList<GitHubIssueCrawlerConfig> GitHubIssuesToCrawl { get; set; }

    }
}