using System.Diagnostics;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Sloader.Engine.Crawler.Feed
{
    /// <summary>
    /// Implementation for the feedloading
    /// </summary>
    public class SyndicationFeedAbstraction : ISyndicationFeedAbstraction
    {

        /// <summary>
        /// We just can load a feed by using this method
        /// </summary>
        /// <param name="url">Url of the ATOM/RSS feed</param>
        /// <returns>FeedInformation</returns>
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