using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Services.Strategies;

public interface IChangeDetectionStrategyFactory
{
    IChangeDetectionStrategy Get(ChangeType type);
}
