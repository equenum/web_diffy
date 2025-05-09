using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Services.Detection.Strategies;

public interface IChangeDetectionStrategyFactory
{
    IChangeDetectionStrategy Get(ChangeType type);
}
