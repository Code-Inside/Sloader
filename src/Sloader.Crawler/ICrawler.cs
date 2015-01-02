using System.Threading.Tasks;

namespace Sloader.Crawler
{
    public interface ICrawler<T>
    {
        Task<T> DoWorkAsync();
    }
}