using System.Threading.Tasks;
using Sloader.Config.Drop.File;
using Sloader.Result;

namespace Sloader.Engine.Drop.File
{
    public class FileDrop : IDrop<FileDropConfig>
    {
        public Task DoWorkAsync(FileDropConfig config, CrawlerRun crawlerRun)
        {
            return Task.Factory.StartNew(() => System.IO.File.WriteAllText(config.FilePath, crawlerRun.ToJson()));
        }
    }
}