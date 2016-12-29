using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sloader.Result.Types
{
    public class TwitterUserResult : BaseResult
    {
        public override KnownResultType ResultType
        {
            get { return KnownResultType.TwitterUser; }
        }

        public class User : IHaveRawContent
        {
            [JsonProperty("id_str")]
            public string Id { get; set; }

            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("followers_count")]
            public int FollowersCount { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            public string RawContent { get; set; }
        }

        public List<User> Users { get; set; }
    }
}