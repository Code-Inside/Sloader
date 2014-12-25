using System.ServiceModel.Syndication;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public interface IFeedLoader
    {
        SyndicationFeed Get(string url);
    }
}