using System.Diagnostics;
using System.Threading.Tasks;
using Sloader.Config.Crawler.Twitter;
using Sloader.Config.Drop.File;
using Sloader.Result;

namespace Sloader.Engine.Drop.File
{
    public class FileDrop : IDrop<FileDropConfig>
    {
        public Task DoWorkAsync(FileDropConfig config, CrawlerRun crawlerRun)
        {
            Trace.TraceInformation($"{nameof(FileDrop)} dropping stuff at '{config.FilePath}'");

            return Task.Factory.StartNew(() => System.IO.File.WriteAllText(config.FilePath, crawlerRun.ToJson()));
        }
    }
}