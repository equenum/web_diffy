using System;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Strategies;

public class SnapshotChangeDetectionStrategy : IChangeDetectionStrategy
{
    public bool CanHandle(ChangeType type) => type == ChangeType.Snapshot;

    public void Execute(string html, TargetContext context)
    {
        throw new NotImplementedException();
    }
}
