using System.Collections.Generic;
using Sloader.Config.Drop.File;

namespace Sloader.Config
{
    public class DropConfig
    {
        public IList<FileDropConfig> FileDrops { get; set; }
    }
}