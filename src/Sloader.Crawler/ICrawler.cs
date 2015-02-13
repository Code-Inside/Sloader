using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Sloader.Crawler
{
    public interface ICrawler<T>
    {
        Task<T> DoWorkAsync(string resultIdentifier);
    }
}