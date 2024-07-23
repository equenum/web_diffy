using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Api.Services;

public class MonitorJobService : IMonitorJobService
{
    private readonly ILogger<MonitorJobService> _logger;
    private readonly StdSchedulerFactory _schedulerFactory;

    public MonitorJobService(ILogger<MonitorJobService> logger)
    {
        _logger = logger;
        _schedulerFactory = new StdSchedulerFactory();
    }

    public async Task ScheduleAsync(Target target, CancellationToken cancellationToken = default)
    {
        // todo throw if null

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobDetails = BuildJobDetails(target);
        await scheduler.ScheduleJob(jobDetails.Details, jobDetails.Trigger, cancellationToken);
    }

    public async Task ScheduleAsync(IEnumerable<Target> targets, CancellationToken cancellationToken = default)
    {
        // todo throw in empty

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        foreach (var details in targets.Select(BuildJobDetails))
        {
            await scheduler.ScheduleJob(details.Details, details.Trigger, cancellationToken);
            _logger.LogInformation("Scheduled job {JobKey} for target {TargetName}.",
                details.Details.Key,
                details.JobTargetName);
        }
    }

    public Task UnscheduleByResourceAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UnscheduleByTargetAsync(Guid targetId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static JobDetailsBundle BuildJobDetails(Target target)
    {
        var jobDetails = JobBuilder.Create<MonitorChangeJob>()
            .WithIdentity(target.Id.ToString(), target.ResourceId.ToString())
            // todo extract to constant
            .UsingJobData("target-context", JsonSerializer.Serialize(target.ToTargetContext()))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(target.Id.ToString(), target.ResourceId.ToString())
            .StartNow()
            .WithCronSchedule(target.CronSchedule)
            .Build();

        return new JobDetailsBundle(jobDetails, trigger, target.DisplayName);
    }
}
