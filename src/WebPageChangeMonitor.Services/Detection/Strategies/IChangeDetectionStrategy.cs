using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Detection.Strategies;

public interface IChangeDetectionStrategy
{
    bool CanHandle(ChangeType type);
    Task ExecuteAsync(string html, TargetContext context);
}
