namespace Sloader.Shared.Feed
{
    public class FeedCrawlerConfig : BaseCrawlerConfig
    {
        public override string ResultIdentifier
        {
            get { return Url; }
        }

        public string Url { get; set; }
        public bool LoadSocialLinkCounters { get; set; }
        public bool LoadFullContent { get; set; }

    }
}