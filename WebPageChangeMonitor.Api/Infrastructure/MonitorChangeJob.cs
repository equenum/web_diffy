using System;
using System.Threading.Tasks;
using Quartz;

namespace WebPageChangeMonitor.Api.Infrastructure;

public class MonitorChangeJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        var url = dataMap.GetString("url");

        Console.WriteLine($"Executing job {context.JobDetail.Key}, url: {url}");
    }
}
