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
    ///   - FilePath: "test1.json"
    ///   - FilePath: "test2.json"
    ///   - FilePath: "test3.json"
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