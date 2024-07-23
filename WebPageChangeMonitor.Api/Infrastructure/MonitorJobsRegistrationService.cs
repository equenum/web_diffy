using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IJobFactory _jobFactory;
    private readonly IMonitorJobService _jobService;

    public MonitorJobsRegistrationService(
        ILogger<MonitorJobsRegistrationService> logger,
        IServiceScopeFactory scopeFactory,
        IJobFactory jobFactory,
        IMonitorJobService jobService)
    {
        _logger = logger;
        _schedulerFactory = new StdSchedulerFactory();
        _scopeFactory = scopeFactory;
        _jobFactory = jobFactory;
        _jobService = jobService;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();
            var targetEntities = await dbContext.Targets.ToListAsync(cancellationToken);

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
