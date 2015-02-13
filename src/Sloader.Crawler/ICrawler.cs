using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Sloader.Crawler
{
    public interface ICrawler<TResult, TConfig>
    {
        Task<TResult> DoWorkAsync(TConfig config);
    }
}