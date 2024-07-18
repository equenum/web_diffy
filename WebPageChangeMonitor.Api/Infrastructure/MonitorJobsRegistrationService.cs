using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api.Infrastructure;

public class MonitorJobsRegistrationService : IHostedService
{
    private readonly ILogger<MonitorJobsRegistrationService> _logger;
    private readonly StdSchedulerFactory _schedulerFactory;
    private readonly ChangeMonitorOptions _options;
    private readonly IJobFactory _jobFactory;

    public MonitorJobsRegistrationService(
        ILogger<MonitorJobsRegistrationService> logger,
        IOptions<ChangeMonitorOptions> options,
        IJobFactory jobFactory)
    {
        _logger = logger;
        _schedulerFactory = new StdSchedulerFactory();
        _options = options.Value;
        _jobFactory = jobFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // todo replace with a database call
        // var monitoringTargets = _options.Resources.SelectMany(resource => resource.Targets);
        // var jobDetails = monitoringTargets.Select(BuildJobDetails);
        // _logger.LogInformation($"JobCount: {jobDetails.Count()}");

        // var scheduler = await _schedulerFactory.GetScheduler();
        // scheduler.JobFactory = _jobFactory;

        // await scheduler.Start();

        // foreach (var details in jobDetails)
        // {
        //     await scheduler.ScheduleJob(details.Details, details.Trigger);
        // }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Shutdown();
    }

    private static JobDetailsBundle BuildJobDetails(Target target)
    {
        // todo use auto mapper
        var context = new TargetContext()
        {
            Url = target.Url,
            ChangeType = target.ChangeType,
            HtmlTag = target.HtmlTag,
            SelectorType = target.SelectorType,
            SelectorValue = target.SelectorValue,
            ExpectedValue = target.ExpectedValue 
        };

        var jobDetails = JobBuilder.Create<MonitorChangeJob>()
            .WithIdentity(target.Id.ToString())

            // extract const
            .UsingJobData("target-context", JsonSerializer.Serialize(context))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{target.Id}-trigger") // extract const
            .StartNow()
            .WithCronSchedule(target.CronSchedule)
            .Build();

        return new JobDetailsBundle(jobDetails, trigger);
    }
}
