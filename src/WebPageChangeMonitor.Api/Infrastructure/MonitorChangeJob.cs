using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Quartz;
using UUIDNext;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api.Infrastructure;

[DisallowConcurrentExecution]
public class MonitorChangeJob : IJob
{
    private readonly ILogger<MonitorChangeJob> _logger;
    private readonly IChangeDetector _changeDetector;
    private readonly ChangeMonitorOptions _options;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;

    public MonitorChangeJob(
        ILogger<MonitorChangeJob> logger,
        IChangeDetector changeDetector,
        IOptions<ChangeMonitorOptions> options,
        IDbContextFactory<MonitorDbContext> contextFactory)
    {
        _logger = logger;
        _changeDetector = changeDetector;
        _options = options.Value;
        _contextFactory = contextFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jsonContext = context.JobDetail.JobDataMap.GetString(JobConsts.DataKeys.TargetContext);
        var targetContext = JsonSerializer.Deserialize<TargetContext>(jsonContext);

        _logger.LogInformation("Executing job {JobKey}, url: {TargetUrl}",
            context.JobDetail.Key,
            targetContext.Url);

        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                BackoffType = (DelayBackoffType)_options.BackoffType,
                UseJitter = _options.UseJitter,
                MaxRetryAttempts = _options.JobRetry.MaxAttempts,
                Delay = _options.JobRetry.Delay
            })
            .Build();

        try
        {
            await pipeline.ExecuteAsync(async token =>
                await _changeDetector.ProcessAsync(targetContext), context.CancellationToken);
        }
        catch (Exception)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var latestPreviousSnapshot = await dbContext.TargetSnapshots
                    .Where(snapshot => snapshot.TargetId == targetContext.Id)
                    .OrderByDescending(snapshot => snapshot.CreatedAt)
                    .FirstOrDefaultAsync();

                var failureSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    TargetId = targetContext.Id,
                    Value = latestPreviousSnapshot is not null ? latestPreviousSnapshot.Value : string.Empty,
                    NewValue = latestPreviousSnapshot is not null ? latestPreviousSnapshot.NewValue : string.Empty,
                    IsChangeDetected = false,
                    Outcome = Models.Consts.Outcome.Failure,
                    Message = $"Failed to process {targetContext.ChangeType} change detection.",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(failureSnapshot);
                await dbContext.SaveChangesAsync();
            }

            _logger.LogWarning("Job pipeline failed, job {JobKey}, url: {TargetUrl}, reason: {Reason}",
                context.JobDetail.Key,
                targetContext.Url,
                $"Max. retry attempts reached - {_options.JobRetry.MaxAttempts}");
        }
    }
}
