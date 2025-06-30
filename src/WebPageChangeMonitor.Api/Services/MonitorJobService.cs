using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
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
        ArgumentNullException.ThrowIfNull(target);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobDetails = BuildJobDetails(target);
        await scheduler.ScheduleJob(jobDetails.Details, jobDetails.Trigger, cancellationToken);
    }

    public async Task ScheduleAsync(IEnumerable<Target> targets, CancellationToken cancellationToken = default)
    {
        if (!targets.Any())
        {
            throw new InvalidOperationException("Target collection is empty.");
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        foreach (var details in targets.Select(BuildJobDetails))
        {
            await scheduler.ScheduleJob(details.Details, details.Trigger, cancellationToken);
            _logger.LogInformation("Scheduled job {JobKey} for target {TargetName}.",
                details.Details.Key,
                details.JobTargetName);
        }
    }

    public async Task UnscheduleByResourceAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        var groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(resourceId.ToString());

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKeys = await scheduler.GetTriggerKeys(groupMatcher, cancellationToken);
        if (triggerKeys.Count == 0)
        {
            _logger.LogInformation("Job triggers not found, group name: {TriggerGroup}.",
                resourceId.ToString());

            return;
        }

        await scheduler.UnscheduleJobs(triggerKeys, cancellationToken);
    }

    public async Task UnscheduleByTargetAsync(Guid targetId, Guid resourceId, CancellationToken cancellationToken = default)
    {
        var groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(resourceId.ToString());

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKeys = await scheduler.GetTriggerKeys(groupMatcher, cancellationToken);

        var targetTriggerKey = triggerKeys.FirstOrDefault(key => key.Name == targetId.ToString());
        if (targetTriggerKey is null)
        {
            _logger.LogError("Job trigger not found, key: {TriggerName}.{TriggerGroup}.",
                targetId.ToString(),
                resourceId.ToString());

            throw new InvalidOperationException($"Job trigger not found, key: {targetId}.{resourceId}.");
        }

        await scheduler.UnscheduleJob(targetTriggerKey, cancellationToken);
    }

    private static JobDetailsBundle BuildJobDetails(Target target)
    {
        var jobDetails = JobBuilder.Create<MonitorChangeJob>()
            .WithIdentity(target.Id.ToString(), target.ResourceId.ToString())
            .UsingJobData(JobConsts.DataKeys.TargetContext, JsonSerializer.Serialize(target.ToTargetContext()))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(target.Id.ToString(), target.ResourceId.ToString())
            .StartNow()
            .WithCronSchedule(target.CronSchedule)
            .Build();

        return new JobDetailsBundle(jobDetails, trigger, target.DisplayName);
    }
}
