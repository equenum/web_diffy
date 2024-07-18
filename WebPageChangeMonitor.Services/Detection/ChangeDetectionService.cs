using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Strategies;

namespace WebPageChangeMonitor.Services.Detection;

public class ChangeDetectionService : IChangeDetectionService
{
    private readonly IChangeDetectionStrategyFactory _factory;

    public ChangeDetectionService(IChangeDetectionStrategyFactory factory)
    {
        _factory = factory;
    }

    public Task ProcessAsync(string html, TargetContext context)
    {
        // introduce parsing strategies and factory
        var strategy = _factory.Get(context.ChangeType);
        strategy.Execute(html, context);

        throw new System.NotImplementedException();
    }
}
