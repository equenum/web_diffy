using System.Net.Http;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Detection.Strategies;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IChangeDetectionStrategyFactory _strategyFactory;

    public ChangeDetector(
        IHttpClientFactory clientFactory, 
        IChangeDetectionStrategyFactory strategyFactory)

    {
        _clientFactory = clientFactory;
        _strategyFactory = strategyFactory;
    }

    public async Task ProcessAsync(TargetContext context)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, context.Url);

        var client = _clientFactory.CreateClient();
        var httpResponseMessage = await client.SendAsync(requestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var html = await httpResponseMessage.Content.ReadAsStringAsync();

            var strategy = _strategyFactory.Get(context.ChangeType);
            await strategy.ExecuteAsync(html, context);
        }
    }
}
