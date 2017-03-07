using System.Collections.Generic;
using Sloader.Config.Drop.File;
using Sloader.Config.Drop.GitHub;

namespace Sloader.Config.Drop
{
    /// <summary>
    /// Container for all Drop configs
    /// </summary>
    /// <example>
    /// Demo yml config (only structure for Crawlers):
    /// <code>
    /// Drop:
    ///   FileDrops:
    ///   - Key: Blog
    ///     Url: http://blog.codeinside.eu/feed
    ///     LoadSocialLinkCounters: false
    ///   FeedsToCrawl:
    ///   - Key: BlogTwo
    ///     Url: http://blog.codeinside.eu/feed2
    ///     LoadSocialLinkCounters: false
    ///   FeedsToCrawl:
    ///   - Key: BlogThreeCombined
    ///     Url: http://blog.codeinside.eu/feed3;http://blog.codeinside.eu/feed4
    ///     LoadSocialLinkCounters: false
    ///   ...
    /// </code>
    /// </example>
    public class DropConfig
    {
        public DropConfig()
        {
            FileDrops = new List<FileDropConfig>();
            GitHubDrops = new List<GitHubDropConfig>();
        }

        /// <summary>
        /// Represents all FileDrops
        /// </summary>
        public IList<FileDropConfig> FileDrops { get; set; }

        /// <summary>
        /// Represents all GitHubDrops
        /// </summary>
        public IList<GitHubDropConfig> GitHubDrops { get; set; }

    }
}