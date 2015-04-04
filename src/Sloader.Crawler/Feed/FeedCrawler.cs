using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sloader.Crawler.Config;
using Sloader.Crawler.Config.Feed;
using Sloader.Results;

namespace Sloader.Crawler.Feed
{
    public class FeedCrawler : ICrawler<FeedCrawlerResult, FeedCrawlerConfig>
    {
        private readonly ISyndicationFeedAbstraction _feedAbstraction;
        private readonly ITwitterTweetCountLoader _twitterLoader;
        private readonly IFacebookShareCountLoader _facebookLoader;

        public FeedCrawler()
            : this(new SyndicationFeedAbstraction(), new TwitterTweetCountLoader(), new FacebookShareCountLoader())
        {

        }

        public FeedCrawler(ISyndicationFeedAbstraction syndicationFeedAbstraction, ITwitterTweetCountLoader twitterLoader, IFacebookShareCountLoader facebookLoader)
        {
            _feedAbstraction = syndicationFeedAbstraction;
            _twitterLoader = twitterLoader;
            _facebookLoader = facebookLoader;
        }


        public async Task<FeedCrawlerResult> DoWorkAsync(FeedCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Url))
                return new FeedCrawlerResult();

            var crawlerResult = new FeedCrawlerResult();
            crawlerResult.ResultIdentifier = config.ResultIdentifier;
            crawlerResult.FeedItems = new List<FeedCrawlerResult.FeedItem>();

            var syndicationFeed = _feedAbstraction.Get(config.Url);

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

                if (config.LoadSocialLinkCounters)
                {
                    crawlerResultItem.TweetsCount = await _twitterLoader.GetAsync(feedItem.Id);
                    crawlerResultItem.FacebookCount = await _facebookLoader.GetAsync(feedItem.Id);
                }
                
                crawlerResultItem.CommentsCount = commentCount;

                if (feedItem.Summary != null)
                {
                    crawlerResultItem.Summary = feedItem.Summary.Text;
                }

                if (feedItem.Links.Any())
                {
                    crawlerResultItem.Href = feedItem.Links.First().Uri.ToString();
                }
                else
                {
                    crawlerResultItem.Href = feedItem.Id;
                }

                crawlerResultItem.PublishedOn = feedItem.PublishDate.Date;

                crawlerResult.FeedItems.Add(crawlerResultItem);
            }

            return crawlerResult;
        }
    }
}
