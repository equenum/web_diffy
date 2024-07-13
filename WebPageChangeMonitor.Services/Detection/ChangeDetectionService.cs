using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Change;
using WebPageChangeMonitor.Services.Parsers;
using WebPageChangeMonitor.Services.Strategies;

namespace WebPageChangeMonitor.Services.Detection;

public class ChangeDetectionService : IChangeDetectionService
{
    private readonly IChangeDetectionStrategyFactory _factory;
    private readonly IHtmlParser _htmlParser;

    public ChangeDetectionService(
        IChangeDetectionStrategyFactory factory,
        IHtmlParser htmlParser)
    {
        _factory = factory;
        _htmlParser = htmlParser;
    }

    public Task ProcessAsync(string html, TargetContext context)
    {
        // introduce parsing strategies and factory
        var text = _htmlParser.GetNodeInnerText(html, context);
        var strategy = _factory.Get(context.ChangeType);

        throw new System.NotImplementedException();
    }
}
