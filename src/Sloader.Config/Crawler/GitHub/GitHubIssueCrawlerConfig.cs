namespace Sloader.Config.Crawler.GitHub
{
    /// <inheritdoc />
    /// <summary>
    /// Crawler config to read GitHubIssues (PRs and Issues)
    /// <para>Define a repo, user or organization and sloader will load the desired issue data..</para>
    /// <para>If you want, you can configure a repository, a user and a org under one key and it will be merged into one list.</para>
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// GitHubIssuesToCrawl:
    /// - Repository: code-inside/sloader
    ///   Key: GitHubEventsRepo
    /// - Organization: code-inside
    ///   Key: GitHubEventsOrg
    /// - User: robertmuehsig;oliverguhr
    ///   Key: GitHubEventsUser
    /// </code>
    /// </example>
    public class GitHubIssueCrawlerConfig : BaseCrawlerConfig
    {
        /// <summary>
        /// Loads Issues based on a given repository in the form of USER/REPO or ORG/REPO.
        /// <para>It is allowed to use a ; to load multiple repository events under one key.</para>
        /// <para>Uses a endpoint like this https://api.github.com/repos/code-inside/sloader/events </para>
        /// </summary>
        /// <example>code-inside/sloader;robertmuehsig/EinKofferVollerReisen</example>
        public string Repository { get; set; }
    }
}