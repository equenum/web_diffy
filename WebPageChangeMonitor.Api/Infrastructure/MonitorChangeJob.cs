using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Api.Infrastructure;

[DisallowConcurrentExecution]
public class MonitorChangeJob : IJob
{
    private readonly ILogger<MonitorChangeJob> _logger;
    private readonly IChangeDetector _changeDetector;

    public MonitorChangeJob(
        ILogger<MonitorChangeJob> logger,
        IChangeDetector changeDetector)
    {
        _logger = logger;
        _changeDetector = changeDetector;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        var jsonContext = dataMap.GetString("target-context");
        var targetContext =  JsonSerializer.Deserialize<TargetContext>(jsonContext);

        _logger.LogInformation("Executing job {JobKey}, url: {TargetUrl}",
            context.JobDetail.Key,
            targetContext.Url);

        // pass arguments
        // add exception handling
        await _changeDetector.ProcessAsync(targetContext);
    }
}
