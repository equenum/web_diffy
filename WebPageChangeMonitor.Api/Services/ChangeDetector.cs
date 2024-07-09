using System.Net.Http;
using System.Threading.Tasks;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IHtmlParser _htmlParser;

    public ChangeDetector(
        IHttpClientFactory clientFactory,
        IHtmlParser htmlParser)
    {
        _clientFactory = clientFactory;
        _htmlParser = htmlParser;
    }
    
    public async Task ProcessAsync(string url)
    {
        // add Polly

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
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

            // introduce parsing strategies and factory
            
            // Html Agility Pack (HAP):
            //  - https://html-agility-pack.net/
            //  - https://github.com/zzzprojects/html-agility-pack

            var text = _htmlParser.GetNodeInnerText(html);
        }
    }
}
