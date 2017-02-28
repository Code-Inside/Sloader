using System;
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
            public string Id { get; set; }

            public DateTime CreatedAt { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public int FollowersCount { get; set; }

            public string Description { get; set; }

            public string RawContent { get; set; }
        }

        public List<User> Users { get; set; }
    }
}