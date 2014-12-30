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
        private readonly ITwitterTweetCountLoader _twitterLoader;
        private readonly IFacebookShareCountLoader _facebookLoader;

        public FeedCrawler() : this(new FeedLoader(), new TwitterTweetCountLoader(), new FacebookShareCountLoader())
        {

        }

        public FeedCrawler(IFeedLoader loader, ITwitterTweetCountLoader twitterLoader, IFacebookShareCountLoader facebookLoader)
        {
            _feedLoader = loader;
            _twitterLoader = twitterLoader;
            _facebookLoader = facebookLoader;
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
                    
                    var crawlerResultItem = new FeedCrawlerResult.FeedItem();
                    crawlerResultItem.Title = feedItem.Title.Text;

                    crawlerResultItem.TweetsCount = await _twitterLoader.GetAsync(feedItem.Id);
                    crawlerResultItem.FacebookCount = await _facebookLoader.GetAsync(feedItem.Id);

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
