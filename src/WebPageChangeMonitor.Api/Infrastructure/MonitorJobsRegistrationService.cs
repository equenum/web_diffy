using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Quartz.Spi;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Common.Stats;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Api.Infrastructure;

public class MonitorJobsRegistrationService : IHostedService
{
    private readonly ILogger<MonitorJobsRegistrationService> _logger;
    private readonly StdSchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly IMonitorJobService _jobService;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;

    public MonitorJobsRegistrationService(
        ILogger<MonitorJobsRegistrationService> logger,
        IJobFactory jobFactory,
        IMonitorJobService jobService,
        IDbContextFactory<MonitorDbContext> contextFactory)
    {
        _logger = logger;
        _schedulerFactory = new StdSchedulerFactory();
        _jobFactory = jobFactory;
        _jobService = jobService;
        _contextFactory = contextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitiateScheduler(cancellationToken);

        using (var context = _contextFactory.CreateDbContext())
        {
            var targetEntities = await context.Targets.Where(target => target.State == State.Active).ToListAsync(cancellationToken);

            if (targetEntities.Count > 0)
            {
                _logger.LogInformation("Existing active targets found, count: {TargetCount}. Scheduling jobs...",
                    targetEntities.Count);

                var targets = targetEntities.Select(entity => entity.ToTarget());

                foreach (var target in targets)
                {
                    await _jobService.ScheduleAsync(target, cancellationToken);

                    _logger.LogInformation("Scheduled a job, target id: {TargetId}, url: {TargetUrl}.",
                        target.Id,
                        target.Url);
                    
                    MonitorMetrics.ActiveTargets.Inc();

                    var jitterDelay = Random.Shared.Next(500, 5000);
                    await Task.Delay(jitterDelay, cancellationToken);
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.Shutdown(cancellationToken);

        MonitorMetrics.ActiveTargets.DecTo(0);
    }

    private async Task InitiateScheduler(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        scheduler.JobFactory = _jobFactory;

        await scheduler.Start(cancellationToken);
        MonitorMetrics.ActiveTargets.DecTo(0);
    }
}
