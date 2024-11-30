using System;
using System.Collections.Generic;
using System.Linq;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Services.Detection.Strategies;

public class ChangeDetectionStrategyFactory : IChangeDetectionStrategyFactory
{
    private readonly IEnumerable<IChangeDetectionStrategy> _strategies;

    public ChangeDetectionStrategyFactory(IEnumerable<IChangeDetectionStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IChangeDetectionStrategy Get(ChangeType type)
    {
        var strategy = _strategies.FirstOrDefault(str => str.CanHandle(type));
        if (strategy is null)
        {
            throw new InvalidOperationException($"Unexpected change type value: {type}");
        }

        return strategy;
    }
}
