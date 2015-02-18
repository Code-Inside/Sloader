using System.ServiceModel.Syndication;

namespace Sloader.Crawler.Feed
{
    public interface ISyndicationFeedAbstraction
    {
        SyndicationFeed Get(string url);
    }
}