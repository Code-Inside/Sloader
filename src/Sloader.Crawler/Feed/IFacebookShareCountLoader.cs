using System.Threading.Tasks;

namespace Sloader.Crawler.Feed
{
    public interface IFacebookShareCountLoader
    {
        Task<int> GetAsync(string url);
    }
}