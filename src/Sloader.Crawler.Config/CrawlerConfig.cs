using System.Collections.Generic;
using Sloader.Crawler.Config.Feed;
using Sloader.Crawler.Config.Twitter;

namespace Sloader.Crawler.Config
{
    public class CrawlerConfig
    {
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; } 
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; } 
    }
}