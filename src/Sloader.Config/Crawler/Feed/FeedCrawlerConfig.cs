namespace Sloader.Config.Crawler.Feed
{
    /// <summary>
    /// Crawler config to read ATOM/RSS-feeds
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// FeedsToCrawl:
    /// - Key: Blog
    ///   Url: https://blog.codeinside.eu/feed
    ///   LoadSocialLinkCounters: false
    /// - Key: GitHub
    ///   Url: https://github.com/robertmuehsig.atom; https://github.com/oliverguhr.atom
    ///   LoadSocialLinkCounters: false
    /// </code>
    /// </example>
    public class FeedCrawlerConfig : BaseCrawlerConfig
    {
        public FeedCrawlerConfig()
        {
            LoadSocialLinkCounters = false;
        }

        /// <summary>
        /// Absolute links to ATOM/RSS-feed.
        /// <para>It is allowed to use a ; to load multiple feeds under one key.</para>
        /// </summary>
        /// <example>https://blog.codeinside.eu/feed/;https://github.com/robertmuehsig.atom</example>
        public string Url { get; set; }

        /// <summary>
        /// Try to load SocialLinkCounters, e.g. Facebook Likes.
        /// <para>Default is false.</para>
        /// </summary>
        public bool LoadSocialLinkCounters { get; set; }
    }
}