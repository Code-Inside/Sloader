using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    /// <inheritdoc />
    /// <summary>
    /// Twitter User Lookup Result for given Twitter Account
    /// <para>Twitter API https://dev.twitter.com/rest/reference/get/users/lookup</para>
    /// </summary>
    public class TwitterUserResult : BaseResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Defined ResultType
        /// </summary>
        public override KnownResultType ResultType
        {
            get { return KnownResultType.TwitterUser; }
        }

        /// <summary>
        /// Actual Twitter User type
        /// </summary>
        public class User : IHaveRawContent
        {
            /// <summary>
            /// UserId on Twitter
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Creation Date for this Twitter Account
            /// </summary>
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Twitter Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Given URL for this Twitter Account
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Twitter Followers count
            /// </summary>
            public int FollowersCount { get; set; }

            /// <summary>
            /// Given description for this Twitter Account
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// JSON serialized complete twitter user data
            /// </summary>
            public string RawContent { get; set; }
        }

        /// <summary>
        /// Collection for all twitter user lookups
        /// </summary>
        public List<User> Users { get; set; }
    }
}