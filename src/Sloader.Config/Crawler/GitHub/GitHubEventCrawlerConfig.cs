using System.Collections.Generic;

namespace Sloader.Config.Crawler.GitHub
{
    public class GitHubEventCrawlerConfig : BaseCrawlerConfig
    {
        public string Repository { get; set; }
        public string Organization { get; set; }
        public string User { get; set; }

    }
}