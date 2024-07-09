using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using WebPageChangeMonitor.Api.Services;

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
        var url = dataMap.GetString("url");

        _logger.LogInformation($"Executing job {context.JobDetail.Key}, url: {url}");

        // pass arguments
        // add exception handling
        await _changeDetector.ProcessAsync(url);
    }
}
