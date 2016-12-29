using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    public class FeedResult : BaseResult
    {
        public class FeedItem : IHaveRawContent
        {
            public string Title { get; set; }
            public DateTime PublishedOn { get; set; }
            public int CommentsCount { get; set; }
            public int FacebookCount { get; set; }
            public string Summary { get; set; }
            public string Href { get; set; }
            public string RawContent { get; set; }
        }

        public List<FeedItem> FeedItems { get; set; }

        public override KnownResultType ResultType
        {
            get { return KnownResultType.Feed; }
        }
    }
}