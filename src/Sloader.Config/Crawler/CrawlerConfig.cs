using System.Collections.Generic;
using Sloader.Config.Crawler.Feed;
using Sloader.Config.Crawler.GitHub;
using Sloader.Config.Crawler.Twitter;

namespace Sloader.Config.Crawler
{
    /// <summary>
    /// Container for all Crawler configs
    /// </summary>
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