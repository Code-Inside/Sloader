﻿using System;
using System.Collections.Generic;

namespace Sloader.Results
{
    public class FeedCrawlerResult : BaseCrawlerResult
    {
        public class FeedItem
        {
            public string Title { get; set; }
            public DateTime PublishedOn { get; set; }
            public int CommentsCount { get; set; }
            public int TweetsCount { get; set; }
            public int FacebookCount { get; set; }
            public string Summary { get; set; }
            public string Href { get; set; }
        }

        public List<FeedItem> FeedItems { get; set; }
    }
}