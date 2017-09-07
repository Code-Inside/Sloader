namespace Sloader.Config.Crawler
{
    /// <summary>
    /// Base of all crawler configs
    /// </summary>
    public abstract class BaseCrawlerConfig
    {
        /// <summary>
        /// Each crawler can be used n-times, but each key must be unique to store the result set.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Each crawler can embed the actual raw result in their result type. Set to true if you want to parse the data yourself.
        /// <para>Warning: This will create pretty large result-sets.</para>
        /// </summary>
        public bool IncludeRawContent { get; set; }
    }
}