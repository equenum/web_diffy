using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Change;

namespace WebPageChangeMonitor.Api.Services;

public interface IChangeDetector
{
    Task ProcessAsync(TargetContext context);
}
