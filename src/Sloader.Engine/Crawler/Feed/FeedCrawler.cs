using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sloader.Config.Crawler.Feed;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Feed
{
    /// <summary>
    /// Atom/Rss-Feed Crawler implementation
    /// </summary>
    public class FeedCrawler : ICrawler<FeedResult, FeedCrawlerConfig>
    {
        private readonly HttpClient _httpClient;
        private readonly IFacebookShareCountLoader _facebookLoader;

        /// <summary>
        /// Ctor, with default dependencies injected.
        /// </summary>
        public FeedCrawler()
        {
            _httpClient = new HttpClient();
            _facebookLoader = new FacebookShareCountLoader();
        }

        /// <summary>
        /// Ctor for test
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        /// <param name="facebookLoader">Loader for Facebook Likes</param>
        public FeedCrawler(FakeHttpMessageHandler messageHandler, IFacebookShareCountLoader facebookLoader)
        {
            _httpClient = new HttpClient(messageHandler);
            _facebookLoader = facebookLoader;
        }

        /// <summary>
        /// Actual work method to load the feed data.
        /// </summary>
        /// <param name="config">Crawler Config</param>
        /// <returns>FeedResult for the given config data</returns>
        public async Task<FeedResult> DoWorkAsync(FeedCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Url))
                return new FeedResult();

            Trace.TraceInformation($"{nameof(FeedCrawler)} loading stuff for '{config.Url}'");

            var crawlerResult = new FeedResult();
            crawlerResult.FeedItems = new List<FeedResult.FeedItem>();

            var maybeSplittedUrls = config.Url.Split(';');

            foreach (var maybeSplittedUrl in maybeSplittedUrls)
            {

                var response = await _httpClient.GetAsync(maybeSplittedUrl.Trim());

                response.EnsureSuccessStatusCode();

                string rssOrAtomResult = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(rssOrAtomResult))
                {
                    XDocument doc = XDocument.Parse(rssOrAtomResult);

                    if (doc.Root != null && doc.Root.Name.LocalName.ToLowerInvariant() == "rss")
                    {
                        // RSS/Channel/item
                        await ParseRssFeed(config, doc, crawlerResult);
                    }

                    if (doc.Root != null && doc.Root.Name.LocalName.ToLowerInvariant() == "feed")
                    {
                        // Feed/entry
                        await ParseAtomFeed(config, doc, crawlerResult);
                    }
                }

              
                
            }

            crawlerResult.FeedItems = crawlerResult.FeedItems.OrderByDescending(x => x.PublishedOn).ToList();

            return crawlerResult;
        }

        private async Task ParseAtomFeed(FeedCrawlerConfig config, XDocument doc, FeedResult crawlerResult)
        {
            var atomItems = doc.Root.Elements()
                .Where(i => i.Name.LocalName == "entry");
            foreach (var atomItem in atomItems)
            {
                var crawlerResultItem = new FeedResult.FeedItem();
                crawlerResultItem.Title = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "title")?.Value;
                crawlerResultItem.Href = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "link")?.Attribute("href")?.Value;
                crawlerResultItem.Summary = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "content")?.Value;

                var updateDateValue = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "updated")?.Value;
                if (DateTime.TryParse(updateDateValue, out DateTime updateDateDateTime))
                {
                    crawlerResultItem.PublishedOn = updateDateDateTime;
                }
                else
                {
                    var pubDateValue = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "published")?.Value;
                    if (DateTime.TryParse(pubDateValue, out DateTime pubDateDateTime))
                    {
                        crawlerResultItem.PublishedOn = pubDateDateTime;
                    }
                }

               

                if (config.IncludeRawContent)
                {
                    crawlerResultItem.RawContent = atomItem.ToString();
                }

                if (config.LoadSocialLinkCounters)
                {
                    crawlerResultItem.FacebookCount = await _facebookLoader.GetAsync(crawlerResultItem.Href);
                }

                crawlerResult.FeedItems.Add(crawlerResultItem);
            }
        }
        private async Task ParseRssFeed(FeedCrawlerConfig config, XDocument doc, FeedResult crawlerResult)
        {
            var rssItems = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements()
                .Where(i => i.Name.LocalName == "item");
            foreach (var rssItem in rssItems)
            {
                var crawlerResultItem = new FeedResult.FeedItem();
                crawlerResultItem.Title = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "title")?.Value;
                crawlerResultItem.Href = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "link")?.Value;
                crawlerResultItem.Summary = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "description")?.Value;
                var pubDateValue = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "pubDate")?.Value;
                if (DateTime.TryParse(pubDateValue, out DateTime pubDateDateTime))
                {
                    crawlerResultItem.PublishedOn = pubDateDateTime;
                }

                if (config.IncludeRawContent)
                {
                    crawlerResultItem.RawContent = rssItem.ToString();
                }

                var commentValue = rssItem.Elements().FirstOrDefault(i =>
                    i.Name.LocalName == "comments" && i.Name.NamespaceName == "http://purl.org/rss/1.0/modules/slash/")?.Value;
                if (int.TryParse(commentValue, out int commentInt))
                {
                    crawlerResultItem.CommentsCount = commentInt;
                }

                if (config.LoadSocialLinkCounters)
                {
                    crawlerResultItem.FacebookCount = await _facebookLoader.GetAsync(crawlerResultItem.Href);
                }

                crawlerResult.FeedItems.Add(crawlerResultItem);
            }
        }
    }
}
