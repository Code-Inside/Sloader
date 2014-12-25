using System.Diagnostics;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public class FeedLoader : IFeedLoader
    {
        public SyndicationFeed Get(string url)
        {
            Trace.TraceInformation("GetFeed invoked with url: " + url);
            try
            {
                var reader = XmlReader.Create(url);

                var feed = SyndicationFeed.Load(reader);
                return feed;
            }
            catch (WebException exc)
            {
                Trace.TraceError("GetFeed: " + exc.Message);
                return new SyndicationFeed();
            }
        }
    }
}