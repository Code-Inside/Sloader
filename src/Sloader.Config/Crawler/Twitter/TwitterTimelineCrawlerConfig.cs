namespace Sloader.Config.Crawler.Twitter
{
    /// <summary>
    /// Loads tweets from one or more twitter handles.
    /// It is allowed to use a ; to load multiple twitter timelines under one key.
    /// Uses this API https://dev.twitter.com/rest/reference/get/statuses/user_timeline
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// TwitterTimelinesToCrawl:
    /// - Handle: codeinsideblog;robert0muehsig;oliverguhr
    ///   IncludeRetweets: false
    ///   Key: Twitter
    /// </code>
    /// </example>
    public class TwitterTimelineCrawlerConfig : BaseCrawlerConfig
    {
        public TwitterTimelineCrawlerConfig()
        {
            IncludeRetweets = false;
        }

        /// <summary>
        /// Twitter handles. 
        /// It is allowed to use a ; to load timelines under one key.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Defines if Retweets should be included in the timeline.
        /// Default is false.
        /// </summary>
        public bool IncludeRetweets { get; set; }
    }
}