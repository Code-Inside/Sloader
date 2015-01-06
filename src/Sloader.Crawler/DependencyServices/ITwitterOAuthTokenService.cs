using System.Threading.Tasks;

namespace Sloader.Crawler.DependencyServices
{
    public interface ITwitterOAuthTokenService
    {
        Task<string> GetAsync(string consumerKey, string consumerSecret);
    }
}