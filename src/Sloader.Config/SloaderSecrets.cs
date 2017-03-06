namespace Sloader.Config
{
    /// <summary>
    /// Common Secrets that are used for various Crawler/Drops.
    /// Be aware that you can use the $$KEY$$ annotation to replace those values from app.config entries.
    /// </summary>
    /// <example>
    /// As YAML might look light:
    /// Secrets:
    ///   TwitterConsumerKey: $$Sloader.SecretTwitterConsumerKey$$
    ///   TwitterConsumerSecret: $$Sloader.SecretTwitterConsumerSecret$$
    ///   GitHubAccessToken: $$Sloader.SecretGitHubAccessToken$$
    /// </example>
    public class SloaderSecrets
    {
        /// <summary>
        /// Twitter API Consumer Key
        /// </summary>
        public string TwitterConsumerKey { get; set; }

        /// <summary>
        /// Twitter API Consumer Secret
        /// </summary>
        public string TwitterConsumerSecret { get; set; }

        /// <summary>
        /// Checks if Twitter API has needed credentials, check here for more details:
        /// https://apps.twitter.com/
        /// </summary>
        public bool IsTwitterConsumerConfigured
        {
            get
            {
                if (string.IsNullOrEmpty(TwitterConsumerKey))
                    return false;
                if (string.IsNullOrEmpty(TwitterConsumerSecret))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Personal Access Token for GitHub, can be created here:
        /// https://github.com/settings/tokens
        /// </summary>
        public string GitHubAccessToken { get; set; }
    }
}