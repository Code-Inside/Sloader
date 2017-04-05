using System.ServiceModel.Syndication;

namespace Sloader.Engine.Crawler.Feed
{
    /// <summary>
    /// Abstraction to enforce testability
    /// </summary>
    public interface ISyndicationFeedAbstraction
    {
        /// <summary>
        /// We just can load a feed by using this method
        /// </summary>
        /// <param name="url">Url of the ATOM/RSS feed</param>
        /// <returns>FeedInformation</returns>
        SyndicationFeed Get(string url);
    }
}