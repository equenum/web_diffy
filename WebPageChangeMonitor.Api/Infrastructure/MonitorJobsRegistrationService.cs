using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api.Infrastructure;

public class MonitorJobsRegistrationService : IHostedService
{
    private readonly ILogger<MonitorJobsRegistrationService> _logger;
    private readonly StdSchedulerFactory _schedulerFactory;
    private readonly ChangeMonitorOptions _options;

    public MonitorJobsRegistrationService(
        ILogger<MonitorJobsRegistrationService> logger,
        IOptions<ChangeMonitorOptions> options)
    {
        _logger = logger;
        _schedulerFactory = new StdSchedulerFactory();
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // create jobs for all targets
        var monitoringTargets = _options.Resources.SelectMany(resource => resource.Targets);
        var jobDetails = monitoringTargets.Select(BuildJobDetails);
        _logger.LogInformation($"JobCount: {jobDetails.Count()}");

        // schedule jobs
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Start();

        foreach (var details in jobDetails)
        {
            await scheduler.ScheduleJob(details.Details, details.Trigger);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Shutdown();
    }

    private static JobDetailsBundle BuildJobDetails(Target target)
    {
        var jobDetails = JobBuilder.Create<MonitorChangeJob>()
            .WithIdentity(target.Id)

            // todo add all the other data required for polling
            .UsingJobData("url", target.Url)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{target.Id}-trigger")
            .StartNow()
            .WithCronSchedule("0/5 * * * * ?")
            .Build();

        return new JobDetailsBundle(jobDetails, trigger);
    }
}
