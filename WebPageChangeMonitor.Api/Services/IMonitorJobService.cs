using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Api.Services;

public interface IMonitorJobService
{
    Task ScheduleAsync(Target target, CancellationToken cancellationToken = default);
    Task ScheduleAsync(IEnumerable<Target> targets, CancellationToken cancellationToken = default);
    Task UnscheduleByTargetAsync(Guid targetId, CancellationToken cancellationToken = default);
    Task UnscheduleByResourceAsync(Guid resourceId, CancellationToken cancellationToken = default);
}
