using System.ServiceModel.Syndication;

namespace Sloader.Crawler.Feed
{
    public interface IFeedLoader
    {
        SyndicationFeed Get(string url);
    }
}