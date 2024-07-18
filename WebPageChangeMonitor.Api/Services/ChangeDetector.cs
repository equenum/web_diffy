using System.Net.Http;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Detection;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IChangeDetectionService _detectionService;

    public ChangeDetector(
        IHttpClientFactory clientFactory,
        IChangeDetectionService detectionService)
    {
        _clientFactory = clientFactory;
        _detectionService = detectionService;
    }

    public async Task ProcessAsync(TargetContext context)
    {
        // add Polly

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, context.Url)
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
            await _detectionService.ProcessAsync(html, context);
        }
    }
}
