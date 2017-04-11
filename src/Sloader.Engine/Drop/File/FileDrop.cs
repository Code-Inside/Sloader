using System.Diagnostics;
using System.Threading.Tasks;
using Sloader.Config.Drop.File;
using Sloader.Result;

namespace Sloader.Engine.Drop.File
{
    /// <summary>
    /// Will drop the results as a normal text file on disk.
    /// </summary>
    public class FileDrop : IDrop<FileDropConfig>
    {
        /// <summary>
        /// Actual work method - will write everything to disk.
        /// </summary>
        /// <param name="config">Desired file path etc.</param>
        /// <param name="crawlerRun">Drop content.</param>
        /// <returns></returns>
        public Task DoWorkAsync(FileDropConfig config, CrawlerRun crawlerRun)
        {
            Trace.TraceInformation($"{nameof(FileDrop)} dropping stuff at '{config.FilePath}'");

            return Task.Factory.StartNew(() => System.IO.File.WriteAllText(config.FilePath, crawlerRun.ToJson()));
        }
    }
}