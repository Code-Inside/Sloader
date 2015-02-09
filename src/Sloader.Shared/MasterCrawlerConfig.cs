using System.Collections.Generic;

namespace Sloader.Shared
{
    public class MasterCrawlerConfig
    {
       // public IList<FeedCrawlerConfig> FeedCrawlerConfigs { get; set; } 
        public string Feeds { get; set; }
        public string TwitterHandles { get; set; }
       // public IList<TwitterTimelineConfig> TwitterTimelineConfigs { get; set; } 

    }
}