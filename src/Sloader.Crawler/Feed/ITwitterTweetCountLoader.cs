using System.Threading.Tasks;

namespace Sloader.Crawler.Feed
{
    public interface ITwitterTweetCountLoader
    {
        Task<int> GetAsync(string url);
    }
}