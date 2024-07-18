using System;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Strategies;

public class ValueChangeDetectionStrategy : IChangeDetectionStrategy
{
    private readonly IHtmlParser _htmlParser;

    public ValueChangeDetectionStrategy(IHtmlParser htmlParser)
    {
        _htmlParser = htmlParser;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Value;

    public void Execute(string html, TargetContext context)
    {
        // get previous value
        // if first time => save and continue
        // if not first time => compare

        // try get previous value from db
        // get current value
        var currentValue = _htmlParser.GetNodeInnerText(html, context);
        throw new NotImplementedException();
    }
}
