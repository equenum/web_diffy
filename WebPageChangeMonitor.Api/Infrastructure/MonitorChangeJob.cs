using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace WebPageChangeMonitor.Api.Infrastructure;

[DisallowConcurrentExecution]
public class MonitorChangeJob : IJob
{
    private readonly ILogger<MonitorChangeJob> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public MonitorChangeJob(
        ILogger<MonitorChangeJob> logger, 
        IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        var url = dataMap.GetString("url");

        _logger.LogInformation($"Executing job {context.JobDetail.Key}, url: {url}");

        // add Polly
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://github.com/equenum?tab=repositories")
        {
            // Headers =
            // {
            //     { HeaderNames.Accept, "application/vnd.github.v3+json" },
            //     { HeaderNames.UserAgent, "HttpRequestsSample" }
            // }
        };

        var client = _clientFactory.CreateClient();
        var httpResponseMessage = await client.SendAsync(requestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var html = await httpResponseMessage.Content.ReadAsStringAsync();

            // parse html here
            // introduce parsing strategies and factory
        }
    }
}
