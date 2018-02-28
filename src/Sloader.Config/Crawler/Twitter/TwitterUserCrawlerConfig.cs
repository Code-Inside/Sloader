namespace Sloader.Config.Crawler.Twitter
{
    /// <inheritdoc />
    /// <summary>
    /// Loads twitter user info from one twitter handle.
    /// <para>Uses this API https://dev.twitter.com/rest/reference/get/users/lookup </para>
    /// </summary>
    /// <seealso cref="T:Sloader.Config.SloaderSecrets" />
    /// <remarks>SloaderSecrets needed: The TwitterConsumerKey and TwitterConsumerSecret must be configured.</remarks>
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