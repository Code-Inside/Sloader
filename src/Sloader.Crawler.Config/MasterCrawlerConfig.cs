using System.Collections.Generic;
using System.Threading.Tasks;
using Sloader.Crawler.Config.Feed;
using Sloader.Crawler.Config.Twitter;

namespace Sloader.Crawler.Config
{
    public class MasterCrawlerConfig
    {
        public async static Task<MasterCrawlerConfig> Load(string yamlLocation)
        {
            return await MasterCrawlerConfigLoader.GetAsync(yamlLocation);
        }
        public IList<FeedCrawlerConfig> FeedsToCrawl { get; set; } 
        public IList<TwitterTimelineCrawlerConfig> TwitterTimelinesToCrawl { get; set; } 
    }
}