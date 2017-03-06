namespace Sloader.Config.Crawler.Feed
{
    public class FeedCrawlerConfig : BaseCrawlerConfig
    {
        public FeedCrawlerConfig()
        {
            LoadSocialLinkCounters = false;
        }

        public string Url { get; set; }

        public bool LoadSocialLinkCounters { get; set; }
    }
}