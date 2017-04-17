using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{

    /// <summary>
    /// Twitter Timeline Result for given Twitter Account
    /// <para>Twitter API https://dev.twitter.com/rest/reference/get/statuses/user_timeline</para>
    /// </summary>
    public class TwitterTimelineResult : BaseResult
    {

        /// <summary>
        /// Defined ResultType
        /// </summary>
        public override KnownResultType ResultType
        {
            get { return KnownResultType.TwitterTimeline; }
        }

        /// <summary>
        /// Actual Twitter Timeline type
        /// </summary>
        public class Tweet : IHaveRawContent
        {
            /// <summary>
            /// Tweet Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Publishing DateTime
            /// </summary>
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Twitter Screenname, e.g. user1234
            /// </summary>
            public string UserScreenname { get; set; }

            /// <summary>
            /// Tweet Text Content
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Used Twitter Client
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// Number of Retweets
            /// </summary>
            public int RetweetCount { get; set; }

            /// <summary>
            /// Number of Favorites
            /// </summary>
            public int FavoriteCount { get; set; }

            /// <summary>
            /// JSON serialized complete tweet data
            /// </summary>
            public string RawContent { get; set; }
        }

        /// <summary>
        /// Collection for all tweets
        /// </summary>
        public List<Tweet> Tweets { get; set; }
    }
}