namespace Sloader.Config.Drop.GitHub
{
    /// <summary>
    /// Drops the output in a GitHub Repo
    /// </summary>
    /// <seealso cref="SloaderSecrets"/>
    /// <remarks>SloaderSecrets needed: The GitHubAccessToken must be configured.</remarks>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// GitHubDrops:
    /// - Owner: "Code-Inside"
    ///   Repo: "Hub"
    ///   Branch: "gh-pages"
    ///   FilePath: "_data/Sloader.json"
    /// </code>
    /// </example>
    public class GitHubDropConfig
    {
        /// <summary>
        /// Target repository name
        /// </summary>
        public string Repo { get; set; }

        /// <summary>
        /// Repository owners name (can be a user or organization)
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Target branch name in repository
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// target file path to store the actual data
        /// </summary>
        public string FilePath { get; set; }

    }
}