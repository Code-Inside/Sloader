using System.Threading.Tasks;
using Sloader.Result;

namespace Sloader.Engine.Drop
{
    /// <summary>
    /// Common interface for all drops.
    /// </summary>
    /// <typeparam name="TConfig">The configuration type of the drop.</typeparam>
    public interface IDrop<in TConfig>
    {
        /// <summary>
        /// Worker method for each drop.
        /// </summary>
        /// <param name="config">Uses the config to ...</param>
        /// <param name="crawlerRun">... drop the actual crawler run result</param>
        Task DoWorkAsync(TConfig config, CrawlerRun crawlerRun);
    }
}
