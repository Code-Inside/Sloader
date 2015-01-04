using System.Threading.Tasks;

namespace Sloader.Crawler.Twitter
{
    public interface ITwitterOAuthTokenLoader
    {
        Task<string> GetAsync(string consumerKey, string consumerSecret);
    }
}