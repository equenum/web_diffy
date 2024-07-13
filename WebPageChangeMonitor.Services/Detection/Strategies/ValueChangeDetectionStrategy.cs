using System;
using WebPageChangeMonitor.Models.Change;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Services.Strategies;

public class ValueChangeDetectionStrategy : IChangeDetectionStrategy
{
    public bool CanHandle(ChangeType type) => type == ChangeType.Value;

    public void Execute(string html, TargetContext context)
    {
        throw new NotImplementedException();
    }
}
