namespace Sloader.Config.Crawler.Twitter
{
    /// <summary>
    /// Loads twitter user info from one twitter handle.
    /// Uses this API https://dev.twitter.com/rest/reference/get/users/lookup
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// TwitterUsersToCrawl:
    /// - Handle: robert0muehsig
    ///   Key: TwitterUserRobert
    /// - Handle: oliverguhr
    ///   Key: TwitterUserOliver
    /// </code>
    /// </example>
    public class TwitterUserCrawlerConfig : BaseCrawlerConfig
    {
        /// <summary>
        /// Specific twitter handle
        /// </summary>
        public string Handle { get; set; }
    }
}