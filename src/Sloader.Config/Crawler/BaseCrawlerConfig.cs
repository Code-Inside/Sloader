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
    }
}