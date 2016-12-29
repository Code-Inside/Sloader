using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sloader.Config.Crawler.Feed;
using Sloader.Result;
using Sloader.Result.Types;

namespace Sloader.Engine.Crawler.Feed
{
    public class FeedCrawler : ICrawler<FeedResult, FeedCrawlerConfig>
    {
        private readonly ISyndicationFeedAbstraction _feedAbstraction;
        private readonly IFacebookShareCountLoader _facebookLoader;

        public FeedCrawler()
            : this(new SyndicationFeedAbstraction(), new FacebookShareCountLoader())
        {

        }

        public FeedCrawler(ISyndicationFeedAbstraction syndicationFeedAbstraction, IFacebookShareCountLoader facebookLoader)
        {
            _feedAbstraction = syndicationFeedAbstraction;
            _facebookLoader = facebookLoader;
        }


        public async Task<FeedResult> DoWorkAsync(FeedCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Url))
                return new FeedResult();

            var crawlerResult = new FeedResult();
            crawlerResult.FeedItems = new List<FeedResult.FeedItem>();

            var maybeSplittedUrls = config.Url.Split(';');

            foreach (var maybeSplittedUrl in maybeSplittedUrls)
            {
                var syndicationFeed = _feedAbstraction.Get(maybeSplittedUrl.Trim());

                foreach (var feedItem in syndicationFeed.Items)
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

                    var crawlerResultItem = new FeedResult.FeedItem();
                    crawlerResultItem.Title = feedItem.Title.Text;

                    if (config.LoadSocialLinkCounters)
                    {
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

                    StringBuilder builder = new StringBuilder();
                    XmlWriter writer = XmlWriter.Create(builder);
                    feedItem.SaveAsRss20(writer);
                    writer.Close();

                    crawlerResultItem.RawContent = builder.ToString();

                    crawlerResult.FeedItems.Add(crawlerResultItem);
                }
            }

            crawlerResult.FeedItems = crawlerResult.FeedItems.OrderByDescending(x => x.PublishedOn).ToList();

            return crawlerResult;
        }
    }
}
