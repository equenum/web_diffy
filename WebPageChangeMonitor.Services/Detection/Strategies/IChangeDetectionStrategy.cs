using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Strategies;

public interface IChangeDetectionStrategy
{
    bool CanHandle(ChangeType type);
    void Execute(string html, TargetContext context);
}
