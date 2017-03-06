using System.Collections.Generic;
using Sloader.Config.Drop.File;
using Sloader.Config.Drop.GitHub;

namespace Sloader.Config.Drop
{
    /// <summary>
    /// Container for all Drop configs
    /// </summary>
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