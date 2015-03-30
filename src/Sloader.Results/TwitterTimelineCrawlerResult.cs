using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sloader.Results
{
    public class TwitterTimelineCrawlerResult : BaseCrawlerResult
    {
        public override KnownCrawlerResultType ResultType
        {
            get { return KnownCrawlerResultType.TwitterTimeline; }
        }

        public class Tweet
        {
            [JsonProperty("id_str")]
            public string Id { get; set; }

            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("retweet_count")]
            public int RetweetCount { get; set; }

            [JsonProperty("favorite_count")]
            public int FavoriteCount { get; set; }
        }

        public List<Tweet> Tweets { get; set; }
    }
}