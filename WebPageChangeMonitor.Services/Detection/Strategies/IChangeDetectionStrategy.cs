using WebPageChangeMonitor.Models.Change;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Services.Strategies;

public interface IChangeDetectionStrategy
{
    bool CanHandle(ChangeType type);
    void Execute(string html, TargetContext context);
}
