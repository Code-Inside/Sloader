namespace Sloader.Crawler.Config.Twitter
{
    public class TwitterTimelineCrawlerConfig : BaseCrawlerConfig
    {
        public override string ResultIdentifier
        {
            get { return Handle; }
        }

        public string Handle { get; set; }
    }
}