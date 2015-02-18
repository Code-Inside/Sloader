using System.ComponentModel;

namespace Sloader.Crawler.Config.Feed
{
    public class FeedCrawlerConfig : BaseCrawlerConfig
    {
        public FeedCrawlerConfig()
        {
            LoadSocialLinkCounters = true;
        }

        public override string ResultIdentifier
        {
            get { return Url; }
        }

        public string Url { get; set; }

        public bool LoadSocialLinkCounters { get; set; }
    }
}