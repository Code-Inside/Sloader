using System.Collections.Generic;
using Sloader.Config.Crawler.Feed;
using Sloader.Config.Crawler.Twitter;

namespace Sloader.Config
{
    public class CrawlerConfig
    {
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; } 
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; }
        public IList<TwitterUserCrawlerConfig> TwitterUsersToCrawl { get; set; } 
    }
}