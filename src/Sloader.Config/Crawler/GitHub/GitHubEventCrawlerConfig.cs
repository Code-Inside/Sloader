using System.Collections.Generic;

namespace Sloader.Config.Crawler.GitHub
{
    /// <summary>
    /// Crawler config to read GitHubEvents
    /// <para>Define a repo, user or organization and sloader will load the desired event data.</para>
    /// <para>If you want, you can configure a repository, a user and a org under one key and it will be merged into one list.</para>
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// GitHubEventsToCrawl:
    /// - Repository: code-inside/sloader
    ///   Key: GitHubEventsRepo
    /// - Organization: code-inside
    ///   Key: GitHubEventsOrg
    /// - User: robertmuehsig;oliverguhr
    ///   Key: GitHubEventsUser
    /// </code>
    /// </example>
    public class GitHubEventCrawlerConfig : BaseCrawlerConfig
    {
        /// <summary>
        /// Default ctor will set the default event types:
        /// <para>PullRequestEvent and IssuesEvent will be added automatically if Events is not set in the YAML</para>
        /// </summary>
        public GitHubEventCrawlerConfig()
        {
            Events = new List<string>();
            Events.Add("PullRequestEvent");
            Events.Add("IssuesEvent");
        }

        /// <summary>
        /// Loads events based on a given repository in the form of USER/REPO or ORG/REPO.
        /// <para>It is allowed to use a ; to load multiple repository events under one key.</para>
        /// <para>Uses a endpoint like this https://api.github.com/repos/code-inside/sloader/events </para>
        /// </summary>
        /// <example>code-inside/sloader;robertmuehsig/EinKofferVollerReisen</example>
        public string Repository { get; set; }

        /// <summary>
        /// Loads events based on a given org.
        /// <para>It is allowed to use a ; to load multiple organization events under one key.</para>
        /// <para>Uses a endpoint like this https://api.github.com/orgs/code-inside/events </para>
        /// </summary>
        /// <example>code-inside</example>
        public string Organization { get; set; }

        /// <summary>
        /// Loads events based on a given user.
        /// <para>It is allowed to use a ; to load multiple user events under one key.</para>
        /// <para>Uses a endpoint like this https://api.github.com/users/robertmuehsig/events </para>
        /// </summary>
        /// <example>robertmuehsig</example>
        public string User { get; set; }

        /// <summary>
        /// List of all events that will be included.
        /// <para>See https://developer.github.com/v3/activity/events/types// for all GitHubEvent Types</para>
        /// <para>If empty, PullRequestEvent and IssuesEvent will be added automatically</para>
        /// </summary>
        public List<string> Events { get; set; }

    }
}