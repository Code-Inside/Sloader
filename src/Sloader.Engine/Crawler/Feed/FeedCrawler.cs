using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Sloader.Config.Crawler.Feed;
using Sloader.Engine.Util;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Feed
{
    /// <inheritdoc />
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
            _httpClient = SloaderRunner.HttpClient;
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

        /// <inheritdoc />
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

            var crawlerResult = new FeedResult {FeedItems = new List<FeedResult.FeedItem>()};

            var maybeSplittedUrls = config.Url.Split(';');

            foreach (var maybeSplittedUrl in maybeSplittedUrls)
            {

                var response = await _httpClient.GetAsync(maybeSplittedUrl.Trim());

                response.EnsureSuccessStatusCode();

                var rssOrAtomResult = await response.Content.ReadAsStringAsync();

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

        private static string TruncateAtWord(string input, int length)
        {
            if (input == null || input.Length < length)
                return input;

            int iNextSpace = input.LastIndexOf(" ", length, StringComparison.Ordinal);
            return $"{input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim()}...";
        }

        private async Task ParseAtomFeed(FeedCrawlerConfig config, XDocument doc, FeedResult crawlerResult)
        {
            if(doc.Root == null)
                return;

            var atomItems = doc.Root.Elements()
                .Where(i => i.Name.LocalName == "entry");

            bool categoryFilter = config.FilterByCategories.Any();

            foreach (var atomItem in atomItems)
            {
                var crawlerResultItem = new FeedResult.FeedItem
                {
                    Title = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "title")?.Value,
                    Href = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "link")?.Attribute("href")?.Value
                };

                if (categoryFilter)
                {
                    bool doesMatchCategoryFilter = false;

                    var categoryItems = atomItem.Elements().Where(i => i.Name.LocalName == "category");

                    if (categoryItems.Any())
                    {
                        foreach (var categoryItem in categoryItems)
                        {
                            if(categoryItem.Attribute("term") != null)
                            {
                                var categoryValue = categoryItem.Attribute("term").Value.ToCleanString();

                                if (config.FilterByCategories.Contains(categoryValue, StringComparer.OrdinalIgnoreCase))
                                {
                                    doesMatchCategoryFilter = true;
                                    break;
                                }
                            }
                           
                        }
                    }


                    if (doesMatchCategoryFilter == false)
                    {
                        // exit here, because we applied a category filter, but no filter category was present
                        continue;
                    }
                }

                var summary = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "content")?.Value;

                if(summary != null)
                {
                    if (config.SummaryTruncateAt == 0)
                    {
                        crawlerResultItem.Summary = summary;
                    }
                    else
                    {
                        var contentDoc = new HtmlDocument();
                        contentDoc.LoadHtml(summary);
                        var textContent = contentDoc.DocumentNode.InnerText.Trim();
                        crawlerResultItem.Summary = TruncateAtWord(textContent, config.SummaryTruncateAt);
                    }
                }

                var pubDateValue = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "published")?.Value;
                if (DateTime.TryParse(pubDateValue, out DateTime pubDateDateTime))
                {
                    crawlerResultItem.PublishedOn = pubDateDateTime;
                }
                else
                {
                    var updateDateValue = atomItem.Elements().FirstOrDefault(i => i.Name.LocalName == "updated")?.Value;
                    if (DateTime.TryParse(updateDateValue, out DateTime updateDateDateTime))
                    {
                        crawlerResultItem.PublishedOn = updateDateDateTime;
                    }
                }

                XNamespace media = "http://search.yahoo.com/mrss/";
                var mediaGroupElement = atomItem.Element(media + "group");
                if(mediaGroupElement != null)
                {
                    var thumbnailElement = mediaGroupElement.Elements().FirstOrDefault(i => i.Name.LocalName == "thumbnail");
                    if(thumbnailElement != null)
                    {
                        crawlerResultItem.Thumbnail = thumbnailElement.Attribute("url").Value;
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
            if (doc.Root == null)
                return;

            var rssItems = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements()
                .Where(i => i.Name.LocalName == "item");

            bool categoryFilter = config.FilterByCategories.Any();

            foreach (var rssItem in rssItems)
            {
                var crawlerResultItem = new FeedResult.FeedItem
                {
                    Title = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "title")?.Value.ToCleanString(),
                    Href = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "link")?.Value.ToCleanString()
                };

                if(categoryFilter)
                {
                    bool doesMatchCategoryFilter = false;

                    var categoryItems = rssItem.Elements().Where(i => i.Name.LocalName == "category");

                    if (categoryItems.Any())
                    {
                        foreach(var categoryItem in categoryItems)
                        {
                            var categoryValue = categoryItem.Value.ToCleanString();

                            if(config.FilterByCategories.Contains(categoryValue, StringComparer.OrdinalIgnoreCase))
                            {
                                doesMatchCategoryFilter = true;
                                break;
                            }
                        }
                    }


                    if(doesMatchCategoryFilter == false)
                    {
                        // exit here, because we applied a category filter, but no filter category was present
                        continue;
                    }
                }

                var summary = rssItem.Elements().FirstOrDefault(i => i.Name.LocalName == "description")?.Value.ToCleanString();
                if (config.SummaryTruncateAt == 0)
                {
                    crawlerResultItem.Summary = summary;
                }
                else
                {
                    var contentDoc = new HtmlDocument();
                    contentDoc.LoadHtml(summary);
                    var textContent = contentDoc.DocumentNode.InnerText.Trim();
                    crawlerResultItem.Summary = TruncateAtWord(textContent, config.SummaryTruncateAt);
                }

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
