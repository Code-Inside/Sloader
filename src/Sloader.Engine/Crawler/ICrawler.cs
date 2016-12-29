using System.Threading.Tasks;

namespace Sloader.Engine.Crawler
{
    public interface ICrawler<TResult, TConfig>
    {
        Task<TResult> DoWorkAsync(TConfig config);
    }
}