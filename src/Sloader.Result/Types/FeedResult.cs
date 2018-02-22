using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    /// <inheritdoc />
    /// <summary>
    /// Feed Result for RSS/ATOM Feeds
    /// </summary>
    public class FeedResult : BaseResult
    {
        /// <summary>
        /// Actual FeedItem type
        /// </summary>
        public class FeedItem : IHaveRawContent
        {
            /// <summary>
            /// Title Text, e.g. Blogpost title
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Date from FeedItem, e.g. publishing date
            /// </summary>
            public DateTime PublishedOn { get; set; }

            /// <summary>
            /// Some blog systems publish the comment count for this blogpost
            /// <para>It's not defined in the standard, but it is a "known" extension for feeds.</para>
            /// </summary>
            public int CommentsCount { get; set; }

            /// <summary>
            /// If SocialMedia loading is active, Sloader tries to get the current FB-like count for the given blogpost URL.
            /// </summary>
            public int FacebookCount { get; set; }

            /// <summary>
            /// Summary Text, e.g. Blogpost summary
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// URL for the given blogpost etc.
            /// </summary>
            public string Href { get; set; }

            /// <summary>
            /// JSON serialized complete feeditem
            /// </summary>
            public string RawContent { get; set; }
        }

        /// <summary>
        /// Collection for all RSS/ATOM Feed items
        /// </summary>
        public List<FeedItem> FeedItems { get; set; }

        /// <summary>
        /// Defined ResultType
        /// </summary>
        public override KnownResultType ResultType
        {
            get { return KnownResultType.Feed; }
        }
    }
}