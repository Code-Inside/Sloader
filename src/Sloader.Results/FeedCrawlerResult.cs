using System;
using System.Collections.Generic;

namespace Sloader.Results
{
    public class FeedCrawlerResult : BaseCrawlerResult
    {
        public class FeedItem : IHaveRawContent
        {
            public string Title { get; set; }
            public DateTime PublishedOn { get; set; }
            public int CommentsCount { get; set; }
            public int TweetsCount { get; set; }
            public int FacebookCount { get; set; }
            public string Summary { get; set; }
            public string Href { get; set; }
            public string RawContent { get; set; }
        }

        public List<FeedItem> FeedItems { get; set; }

        public override KnownCrawlerResultType ResultType
        {
            get { return KnownCrawlerResultType.Feed; }
        }
    }
}