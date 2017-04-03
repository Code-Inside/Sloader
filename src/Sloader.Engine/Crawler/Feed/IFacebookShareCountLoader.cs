using System.Threading.Tasks;

namespace Sloader.Engine.Crawler.Feed
{
    /// <summary>
    /// Should return for a given url the like count on facebook
    /// </summary>
    public interface IFacebookShareCountLoader
    {
        /// <summary>
        /// Get the likes for a given URL on facebook.com
        /// </summary>
        /// <param name="url">The given url.</param>
        /// <returns>Like-count as task</returns>
        Task<int> GetAsync(string url);
    }
}