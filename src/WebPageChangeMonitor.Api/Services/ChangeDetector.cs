using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.Playwright;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Common.Stats;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Logging;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Detection.Strategies;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IChangeDetectionStrategyFactory _strategyFactory;
    private readonly ILogger<ChangeDetector> _logger;
    private readonly ChangeMonitorOptions _options;

    public ChangeDetector(
        IChangeDetectionStrategyFactory strategyFactory,
        ILogger<ChangeDetector> logger,
        IOptions<ChangeMonitorOptions> options)
    {
        _strategyFactory = strategyFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task ProcessAsync(TargetContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = true
            });

            var browserContext = await browser.NewContextAsync(new() 
            { 
                UserAgent = _options.Playwright.UserAgent,
                ExtraHTTPHeaders = BuildHttpHeaders()
            });

            var page = await browserContext.NewPageAsync();
            var response = await page.GotoAsync(context.Url);

            if (response is null || !response.Ok)
            {
                throw new HtmlParsingException();
            }

            var html = await page.ContentAsync();

            var strategy = _strategyFactory.Get(context.ChangeType);
            await strategy.ExecuteAsync(html, context);

            sw.Stop();
            MonitorMetrics.ChangeDetectionCount.WithLabels(MetricLabels.Success.True, context.ChangeType.ToString()).Inc();
            MonitorMetrics.ChangeDetectionDuration.WithLabels(MetricLabels.Success.True, context.ChangeType.ToString()).Observe(sw.ElapsedMilliseconds);
        }
        catch (HtmlParsingException)
        {
            sw.Stop();
            _logger.LogError("Err-{ErrorCode}: Failed to fetch html page contents for context id '{ContextId}', url '{Url}'.",
                LogErrorCodes.Detection.FetchFailed,
                context.Id,
                context.Url);
            
            MonitorMetrics.ChangeDetectionCount.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Inc();
            MonitorMetrics.ChangeDetectionDuration.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Observe(sw.ElapsedMilliseconds);

            throw;
        }
        catch (Exception)
        {
            sw.Stop();

            _logger.LogError("Err-{ErrorCode}: Failed to process change detection for context id '{ContextId}', url '{Url}'.",
                LogErrorCodes.Detection.Failed,
                context.Id,
                context.Url);

            MonitorMetrics.ChangeDetectionCount.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Inc();
            MonitorMetrics.ChangeDetectionDuration.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Observe(sw.ElapsedMilliseconds);

            throw;
        }
    }

    private Dictionary<string, string> BuildHttpHeaders() => new()
    {
        { HeaderNames.Accept, "*/*" },
        { HeaderNames.AcceptEncoding, "gzip, deflate, br, zstd" },
        { HeaderNames.AcceptLanguage, "en-US,en;q=0.9" },
        { UserHintHeaderNames.Ua, BuildUaUserHint() },
        { UserHintHeaderNames.UaMobile, _options.Playwright.IsMobile ? "?1" : "?0" },
        { UserHintHeaderNames.UaPlatform, $"\"{_options.Playwright.Platform}\"" }
    };

    private string BuildUaUserHint() =>
        $"\"Chromium\";v=\"{_options.Playwright.ChromiumVersion}\", " + 
        $"\"Google Chrome\";v=\"{_options.Playwright.GoogleChromeVersion}\", " + 
        $"\"Not/A)Brand\";v=\"{_options.Playwright.GreaseVersion}\"";
}
