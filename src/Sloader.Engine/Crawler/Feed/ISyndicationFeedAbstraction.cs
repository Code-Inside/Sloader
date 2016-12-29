using System.ServiceModel.Syndication;

namespace Sloader.Engine.Crawler.Feed
{
    public interface ISyndicationFeedAbstraction
    {
        SyndicationFeed Get(string url);
    }
}