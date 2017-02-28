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
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }

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

        public string GitHubAccessToken { get; set; }
    }
}