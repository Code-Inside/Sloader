namespace Sloader.Crawler.Config
{
    public class MasterCrawlerSecrets
    {
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }

        public bool IsTwitterConsumerConfigured
        {
            get
            {
                if (string.IsNullOrEmpty(TwitterConsumerKey))
                    return false;
                if (string.IsNullOrEmpty(TwitterConsumerSecret))
                    return false;

                return true;
            }
        }

    }
}