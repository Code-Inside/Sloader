using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    /// <inheritdoc />
    /// <summary>
    /// Result for the v3 GitHub Event API
    /// <para>https://developer.github.com/v3/activity/events/</para>
    /// </summary>
    public class GitHubEventResult : BaseResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Actual GitHub Event type
        /// </summary>
        public class Event : IHaveRawContent
        {
            /// <summary>
            /// Each GitHubEvent has its own unique ID
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// GitHub Event Type, e.g. CommitEvent or CreateEvent
            /// <para>Full reference: https://developer.github.com/v3/activity/events/types/</para>
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// DateTime when the event happend
            /// </summary>
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Who issued the event
            /// </summary>
            public string Actor { get; set; }

            /// <summary>
            /// Defines the repo on GitHub
            /// </summary>
            public string Repository { get; set; }

            /// <summary>
            /// Defines the org on GitHub
            /// </summary>
            public string Organization { get; set; }

            /// <inheritdoc />
            /// <summary>
            /// JSON serialized complete event data
            /// </summary>
            public string RawContent { get; set; }

            /// <summary>
            /// Based on the <see cref="Type"/> a more descriptive and related action.
            /// <para>e.g. IssueEvent occured and you need to know if the issue is opened, closed etc.</para>
            /// <para>On PullRequests: The closed and IsMerged=true property is handled as merged</para>
            /// </summary>
            public string RelatedAction { get; set; }

            /// <summary>
            /// Based on the <see cref="Type"/> the target url.
            /// </summary>
            public string RelatedUrl { get; set; }

            /// <summary>
            /// Based on the <see cref="Type"/> some more information.
            /// <para>e.g. Issue Title etc.</para>
            /// </summary>
            public string RelatedDescription { get; set; }

            /// <summary>
            /// Based on the <see cref="Type"/> some more information.
            /// <para>e.g. Issue Descritpion etc.</para>
            /// </summary>
            public string RelatedBody { get; set; }

        }

        /// <summary>
        /// Collection for all events
        /// </summary>
        public List<Event> Events { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Defined ResultType
        /// </summary>
        public override KnownResultType ResultType
        {
            get { return KnownResultType.GitHubEvent; }
        }
    }
}