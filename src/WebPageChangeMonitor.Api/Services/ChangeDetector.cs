using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebPageChangeMonitor.Api.Infrastructure.Logging;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Detection.Strategies;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IChangeDetectionStrategyFactory _strategyFactory;
    private readonly ILogger<ChangeDetector> _logger;

    public ChangeDetector(
        IHttpClientFactory clientFactory,
        IChangeDetectionStrategyFactory strategyFactory,
        ILogger<ChangeDetector> logger)

    {
        _clientFactory = clientFactory;
        _strategyFactory = strategyFactory;
        _logger = logger;
    }

    public async Task ProcessAsync(TargetContext context)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, context.Url);

        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var html = await response.Content.ReadAsStringAsync();

                var strategy = _strategyFactory.Get(context.ChangeType);
                await strategy.ExecuteAsync(html, context);
            }
            catch (Exception)
            {
                _logger.LogError("Err-{ErrorCode}: Failed to process change detection for context id '{ContextId}', url '{Url}'.",
                    LogErrorCodes.ChangeDetectionFailed,
                    context.Id,
                    context.Url);

                throw;
            }

            return;
        }

        _logger.LogError("Err-{ErrorCode}: Failed to fetch html page contents for context id '{ContextId}', url '{Url}'.",
            LogErrorCodes.HtmlContentFetchFailed,
            context.Id,
            context.Url);
    }
}
