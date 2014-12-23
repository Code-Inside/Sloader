using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Sloader.Types;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public class FeedCrawler : ICrawler<List<FeedCrawlerResult>>
    {
        public string ConfiguredFeeds { get; set; }
        public async Task<List<FeedCrawlerResult>> DoWorkAsync()
        {
            var results = new List<FeedCrawlerResult>();

            if (string.IsNullOrWhiteSpace(ConfiguredFeeds))
                return results;

            foreach (var feed in ConfiguredFeeds.Split(';'))
            {
                var rssFeed = new FeedCrawlerResult();
                rssFeed.Type = KnownCrawler.Feed;
                rssFeed.Key = feed;
                rssFeed.FeedItems = new List<FeedCrawlerResult.FeedItem>();

                var syndicationFeed = GetFeed(feed);

                foreach (var feedItem in syndicationFeed.Items.OrderBy(x => x.PublishDate))
                {
                    int commentCount = 0;

                    foreach (SyndicationElementExtension extension in feedItem.ElementExtensions)
                    {
                        var extensionElement = extension.GetObject<XElement>();

                        if (extensionElement.Name.LocalName == "comments" &&
                            extensionElement.Name.NamespaceName == "http://purl.org/rss/1.0/modules/slash/")
                        {
                            commentCount = int.Parse(extensionElement.Value);
                        }
                    }

                    var httpClient = new HttpClient();

                    // Get Twitter Counts... with bad code...
                    string twitterUrl = "http://urls.api.twitter.com/1/urls/count.json?url=" + feedItem.Id;

                    var twitterResult = await httpClient.GetAsync(twitterUrl);
                    var jTwitterToken = JToken.Parse(twitterResult.Content.ReadAsStringAsync().Result);
                    var twitterCounter = jTwitterToken.SelectToken("count");

                    // Get Facebook Shares... with bad code..
                    string facebookUrl = "https://graph.facebook.com/?id=" + feedItem.Id;

                    var facebookResult = await httpClient.GetAsync(facebookUrl);
                    var jFacebookToken = JToken.Parse(facebookResult.Content.ReadAsStringAsync().Result);
                    var facebookCounter = jFacebookToken.SelectToken("shares");

                    var rssItem = new FeedCrawlerResult.FeedItem();
                    rssItem.Title = feedItem.Title.Text;
                    if (twitterCounter != null)
                    {
                        rssItem.TweetsCount = twitterCounter.Value<int>();
                    }

                    if (facebookCounter != null)
                    {
                        rssItem.FacebookCount = facebookCounter.Value<int>();
                    }

                    rssItem.CommentsCount = commentCount;

                    if (feedItem.Summary != null)
                    {
                        rssItem.Summary = feedItem.Summary.Text;
                    }

                    rssItem.Href = feedItem.Id;
                    rssItem.PublishedOn = feedItem.PublishDate.Date;

                    rssFeed.FeedItems.Add(rssItem);
                }

                results.Add(rssFeed);
            }

            return results;
        }

        private static SyndicationFeed GetFeed(string url)
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

        public List<FeedCrawlerResult> DoWork()
        {
            throw new NotImplementedException("Use DoWorkAsync.");
        }
    }
}
