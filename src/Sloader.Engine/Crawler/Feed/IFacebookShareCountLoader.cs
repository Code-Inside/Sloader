using System.Threading.Tasks;

namespace Sloader.Engine.Crawler.Feed
{
    public interface IFacebookShareCountLoader
    {
        Task<int> GetAsync(string url);
    }
}