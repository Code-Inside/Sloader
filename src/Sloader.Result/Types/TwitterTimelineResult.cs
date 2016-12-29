using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Sloader.Result.Types
{
    public class TwitterTimelineResult : BaseResult
    {
        public override KnownResultType ResultType
        {
            get { return KnownResultType.TwitterTimeline; }
        }

        public class Tweet : IHaveRawContent
        {
            public string Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string UserScreenname { get; set; }
            public string Text { get; set; }
            public string Source { get; set; }
            public int RetweetCount { get; set; }
            public int FavoriteCount { get; set; }
            public string RawContent { get; set; }
        }

        public List<Tweet> Tweets { get; set; }
    }
}