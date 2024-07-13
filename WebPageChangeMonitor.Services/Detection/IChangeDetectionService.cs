using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Change;

namespace WebPageChangeMonitor.Services.Detection;

public interface IChangeDetectionService
{
    Task ProcessAsync(string html, TargetContext context);
}
