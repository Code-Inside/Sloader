using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Sloader.Types;
using WorldDomination.Net.Http;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public class FeedCrawler : ICrawler<List<FeedCrawlerResult>>
    {
        private readonly IFeedLoader _feedLoader;

        public FeedCrawler() : this(new FeedLoader())
        {

        }

        public FeedCrawler(IFeedLoader loader)
        {
            _feedLoader = loader;
        }

        public string ConfiguredFeeds { get; set; }
        
        public async Task<List<FeedCrawlerResult>> DoWorkAsync()
        {
            var results = new List<FeedCrawlerResult>();

            if (string.IsNullOrWhiteSpace(ConfiguredFeeds))
                return results;

            foreach (var feed in ConfiguredFeeds.Split(';'))
            {
                var crawlerResult = new FeedCrawlerResult();
                crawlerResult.Type = KnownCrawler.Feed;
                crawlerResult.Key = feed;
                crawlerResult.FeedItems = new List<FeedCrawlerResult.FeedItem>();

                var syndicationFeed = _feedLoader.Get(feed);

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

                    // Get Twitter Counts...
                    string twitterUrl = "http://urls.api.twitter.com/1/urls/count.json?url=" + feedItem.Id;
                    string twitterContent;
                    using (var httpClient = HttpClientFactory.GetHttpClient())
                    {
                        // twitterContent sample:
                        // {"count":0,"url":"http://...url..."}
                        twitterContent = await httpClient.GetStringAsync(twitterUrl);
                    }
                    var jTwitterToken = JToken.Parse(twitterContent);
                    var twitterCounter = jTwitterToken.SelectToken("count");

                    // Get Facebook Shares... 
                    string facebookUrl = "https://graph.facebook.com/?id=" + feedItem.Id;
                    string facebookContent;
                    using (var httpClient = HttpClientFactory.GetHttpClient())
                    {
                        // facebookContent sample:
                        // {"id":"http://...url...","shares":1} or just
                        // {"id":"http://...url..."}
                        facebookContent = await httpClient.GetStringAsync(facebookUrl);
                    }
                    var jFacebookToken = JToken.Parse(facebookContent);
                    var facebookCounter = jFacebookToken.SelectToken("shares");

                    var crawlerResultItem = new FeedCrawlerResult.FeedItem();
                    crawlerResultItem.Title = feedItem.Title.Text;
                    if (twitterCounter != null)
                    {
                        crawlerResultItem.TweetsCount = twitterCounter.Value<int>();
                    }

                    if (facebookCounter != null)
                    {
                        crawlerResultItem.FacebookCount = facebookCounter.Value<int>();
                    }

                    crawlerResultItem.CommentsCount = commentCount;

                    if (feedItem.Summary != null)
                    {
                        crawlerResultItem.Summary = feedItem.Summary.Text;
                    }

                    crawlerResultItem.Href = feedItem.Id;
                    crawlerResultItem.PublishedOn = feedItem.PublishDate.Date;

                    crawlerResult.FeedItems.Add(crawlerResultItem);
                }

                results.Add(crawlerResult);
            }

            return results;
        }
    }
}
