using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace WebPageChangeMonitor.Api.Infrastructure;

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
            "https://api.github.com/repos/dotnet/AspNetCore.Docs/branches")
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
            using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
                // parse the response here
                // introduce parsing strategies and factory
            }
        }
    }
}
