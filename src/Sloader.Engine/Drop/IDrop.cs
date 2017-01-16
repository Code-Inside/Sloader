using System.Threading.Tasks;
using Sloader.Result;

namespace Sloader.Engine.Drop
{
    public interface IDrop<TConfig>
    {
        Task DoWorkAsync(TConfig config, CrawlerRun crawlerRun);
    }
}
