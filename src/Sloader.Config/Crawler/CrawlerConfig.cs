using System.Collections.Generic;
using Sloader.Config.Crawler.Feed;
using Sloader.Config.Crawler.Twitter;

namespace Sloader.Config.Crawler
{
    public class CrawlerConfig
    {
        public CrawlerConfig()
        {
            FeedsToCrawl = new List<FeedCrawlerConfig>();
            TwitterTimelinesToCrawl = new List<TwitterTimelineCrawlerConfig>();
            TwitterUsersToCrawl = new List<TwitterUserCrawlerConfig>();

        }
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; } 
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; }
        public IList<TwitterUserCrawlerConfig> TwitterUsersToCrawl { get; set; } 
    }
}