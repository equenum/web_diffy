using System.Threading.Tasks;

namespace WebPageChangeMonitor.Api.Services;

public interface IChangeDetector
{
    Task ProcessAsync(string url);
}
