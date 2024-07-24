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
using WebPageChangeMonitor.Data;

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
        using (var context = _contextFactory.CreateDbContext())
        {
            var targetEntities = await context.Targets.ToListAsync(cancellationToken);

            if (targetEntities.Count > 0) 
            {
                _logger.LogInformation("Existing targets found, count: {TargetCount}. Registering jobs...",
                    targetEntities.Count);

                var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                scheduler.JobFactory = _jobFactory;

                await scheduler.Start(cancellationToken);
                await _jobService.ScheduleAsync(targetEntities.Select(entity => entity.ToTarget()),
                    cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.Shutdown(cancellationToken);
    }
}
