using System.Collections.Generic;
using Sloader.Shared.Feed;
using Sloader.Shared.Twitter;

namespace Sloader.Shared
{
    public class MasterCrawlerConfig
    {
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; } 
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; } 
    }
}