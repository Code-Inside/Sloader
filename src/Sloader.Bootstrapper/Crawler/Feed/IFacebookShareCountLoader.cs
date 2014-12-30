using System.Threading.Tasks;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public interface IFacebookShareCountLoader
    {
        Task<int> GetAsync(string url);
    }
}