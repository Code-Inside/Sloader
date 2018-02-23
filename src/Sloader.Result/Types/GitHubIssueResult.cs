using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    /// <inheritdoc />
    /// <summary>
    /// Result for the v3 GitHub Issue API
    /// <para>https://developer.github.com/v3/issues/#list-issues-for-a-repository</para>
    /// </summary>
    public class GitHubIssueResult : BaseResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Actual GitHub Issue type
        /// </summary>
        public class Issue : IHaveRawContent
        {
            /// <summary>
            /// Each GitHubEvent has its own unique ID
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// True if Issue is related to a Pull Request
            /// </summary>
            public bool IsPullRequest { get; set; }

            /// <summary>
            /// DateTime when the event happend
            /// </summary>
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Who issued the issue
            /// </summary>
            public string Actor { get; set; }

            /// <summary>
            /// Defines the Issue Number on GitHub
            /// </summary>
            public string Number { get; set; }

            /// <summary>
            /// Defines the org on GitHub
            /// </summary>
            public string Organization { get; set; }

            /// <summary>
            /// JSON serialized complete event data
            /// </summary>
            public string RawContent { get; set; }

            /// <summary>
            /// Readable title of the issue or PullRequest
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// State of the PullRequest or Issue, e.g. open or closed
            /// </summary>
            public string State { get; set; }

            /// <summary>
            /// Initial body text of the PullRequest or Issue
            /// </summary>
            public string Body { get; set; }

            /// <summary>
            /// Target URL of the Issue or PullRequest
            /// </summary>
            public string Url { get; set; }
        }

        /// <summary>
        /// Collection for all issues
        /// </summary>
        public List<Issue> Issues { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Defined ResultType
        /// </summary>
        public override KnownResultType ResultType
        {
            get { return KnownResultType.GitHubIssue; }
        }
    }
}