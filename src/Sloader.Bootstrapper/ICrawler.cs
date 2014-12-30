using System.Threading.Tasks;

namespace Sloader.Bootstrapper
{
    public interface ICrawler<T>
    {
        Task<T> DoWorkAsync();
    }
}