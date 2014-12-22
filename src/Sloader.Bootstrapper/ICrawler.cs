using System.Threading.Tasks;

namespace Sloader.Bootstrapper
{
    public interface ICrawler<T>
    {
        T DoWork();
        Task<T> DoWorkAsync();
    }
}