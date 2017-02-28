namespace Sloader.Config.Crawler.Twitter
{
    public class TwitterTimelineCrawlerConfig : BaseCrawlerConfig
    {
        public TwitterTimelineCrawlerConfig()
        {
            IncludeRetweets = false;
        }

        public string Handle { get; set; }

        public bool IncludeRetweets { get; set; }
    }
}