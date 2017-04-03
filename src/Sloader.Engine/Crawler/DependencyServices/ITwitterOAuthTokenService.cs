using System.Threading.Tasks;

namespace Sloader.Engine.Crawler.DependencyServices
{
    /// <summary>
    /// For using the twitter services an OAuth Token is needed.
    /// <para>This Service will fetch the needed token with the given key/secret.</para>
    /// </summary>
    public interface ITwitterOAuthTokenService
    {
        /// <summary>
        /// Get a valid OAuth token to communicate with the Twitter API.
        /// <para>See dev.twitter.com for more information.</para>
        /// </summary>
        /// <param name="consumerKey">Twitter API ConsumerKey</param>
        /// <param name="consumerSecret">Twitter API ConsumerSecret</param>
        /// <returns>Returns the OAuth token as task</returns>
        Task<string> GetAsync(string consumerKey, string consumerSecret);
    }
}