using System.Threading.Tasks;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public interface ITwitterTweetCountLoader
    {
        Task<int> GetAsync(string url);
    }
}