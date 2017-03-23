using System.Threading.Tasks;

namespace Sloader.Engine.Crawler
{
    /// <summary>
    /// Common interface for all crawlers.
    /// </summary>
    /// <typeparam name="TResult">The return type of the crawler.</typeparam>
    /// <typeparam name="TConfig">The configuration type of the crawler.</typeparam>
    public interface ICrawler<TResult, in TConfig>
    {
        /// <summary>
        /// Worker method for each crawler.
        /// </summary>
        /// <param name="config">Uses the specified config...</param>
        /// <returns>... and will return the defined result.</returns>
        Task<TResult> DoWorkAsync(TConfig config);
    }
}