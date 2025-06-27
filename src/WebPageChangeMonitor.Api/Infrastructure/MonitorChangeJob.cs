using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Quartz;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api.Infrastructure;

[DisallowConcurrentExecution]
public class MonitorChangeJob : IJob
{
    private readonly ILogger<MonitorChangeJob> _logger;
    private readonly IChangeDetector _changeDetector;
    private readonly ChangeMonitorOptions _options;

    public MonitorChangeJob(
        ILogger<MonitorChangeJob> logger,
        IChangeDetector changeDetector,
        IOptions<ChangeMonitorOptions> options)
    {
        _logger = logger;
        _changeDetector = changeDetector;
        _options = options.Value;
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

        await pipeline.ExecuteAsync(async token =>
            await _changeDetector.ProcessAsync(targetContext), context.CancellationToken);
    }
}
