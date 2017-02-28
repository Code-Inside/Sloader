using System.Collections.Generic;
using Sloader.Config.Drop.File;
using Sloader.Config.Drop.GitHub;

namespace Sloader.Config.Drop
{
    public class DropConfig
    {
        public DropConfig()
        {
            FileDrops = new List<FileDropConfig>();
            GitHubDrops = new List<GitHubDropConfig>();
        }

        public IList<FileDropConfig> FileDrops { get; set; }

        public IList<GitHubDropConfig> GitHubDrops { get; set; }

    }
}