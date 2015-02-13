using System.ComponentModel;
using YamlDotNet.Serialization;

namespace Sloader.Shared.Feed
{
    public class FeedCrawlerConfig : BaseCrawlerConfig
    {
        public override string ResultIdentifier
        {
            get { return Url; }
        }

        public string Url { get; set; }

        [DefaultValue(true)]
        public bool LoadSocialLinkCounters { get; set; }

        [DefaultValue(true)]
        public bool LoadFullContent { get; set; }

    }
}