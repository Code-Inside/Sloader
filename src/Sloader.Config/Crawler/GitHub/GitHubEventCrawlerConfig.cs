namespace Sloader.Config.Crawler.GitHub
{
    /// <summary>
    /// Crawler config to read GitHubEvents
    /// Define a repo, user or organization and sloader will load the desired event data.
    /// If you want, you can configure a repository, a user and a org under one key and it will be merged into one list.
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
        /// Loads events based on a given repository in the form of USER/REPO or ORG/REPO.
        /// It is allowed to use a ; to load multiple repository events under one key.
        /// Uses a endpoint like this https://api.github.com/repos/code-inside/sloader/events
        /// </summary>
        /// <example>code-inside/sloader;robertmuehsig/EinKofferVollerReisen</example>
        public string Repository { get; set; }

        /// <summary>
        /// Loads events based on a given org.
        /// It is allowed to use a ; to load multiple organization events under one key.
        /// Uses a endpoint like this https://api.github.com/orgs/code-inside/events
        /// </summary>
        /// <example>code-inside</example>
        public string Organization { get; set; }

        /// <summary>
        /// Loads events based on a given user.
        /// It is allowed to use a ; to load multiple user events under one key.
        /// Uses a endpoint like this https://api.github.com/users/robertmuehsig/events
        /// </summary>
        /// <example>robertmuehsig</example>
        public string User { get; set; }

    }
}