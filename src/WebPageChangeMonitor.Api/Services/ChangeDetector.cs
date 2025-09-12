using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UUIDNext;
using WebPageChangeMonitor.Common.Stats;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Logging;
using WebPageChangeMonitor.Services.Detection.Strategies;

namespace WebPageChangeMonitor.Api.Services;

public class ChangeDetector : IChangeDetector
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IChangeDetectionStrategyFactory _strategyFactory;
    private readonly ILogger<ChangeDetector> _logger;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;

    public ChangeDetector(
        IHttpClientFactory clientFactory,
        IChangeDetectionStrategyFactory strategyFactory,
        ILogger<ChangeDetector> logger,
        IDbContextFactory<MonitorDbContext> contextFactory)
    {
        _clientFactory = clientFactory;
        _strategyFactory = strategyFactory;
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task ProcessAsync(TargetContext context)
    {
        var client = _clientFactory.CreateClient();
        var strategy = _strategyFactory.Get(context.ChangeType);

        var isSuccess = false;
        string errorMessage;

        var sw = Stopwatch.StartNew();

        try
        {
            var message = new HttpRequestMessage(HttpMethod.Get, context.Url);
            var response = await client.SendAsync(message);

            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                await strategy.ExecuteAsync(html, context);

                isSuccess = true;

                sw.Stop();
                MonitorMetrics.ChangeDetectionCount.WithLabels(MetricLabels.Success.True, context.ChangeType.ToString()).Inc();
                MonitorMetrics.ChangeDetectionDuration.WithLabels(MetricLabels.Success.True, context.ChangeType.ToString()).Observe(sw.ElapsedMilliseconds);

                return;
            }

            errorMessage = "Failed to fetch html page contents";

            _logger.LogError("Err-{ErrorCode}: Failed to fetch html page contents for context id '{ContextId}', url '{Url}'.",
                LogErrorCodes.Detection.FetchFailed,
                context.Id,
                context.Url);
        }
        catch (Exception ex)
        {
            sw.Stop();
            errorMessage = $"Failed to process {context.ChangeType} change detection: {ex.Message}";

            _logger.LogError("Err-{ErrorCode}: Failed to process change detection for context id '{ContextId}', url '{Url}'.",
                LogErrorCodes.Detection.Failed,
                context.Id,
                context.Url);

            MonitorMetrics.ChangeDetectionCount.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Inc();
            MonitorMetrics.ChangeDetectionDuration.WithLabels(MetricLabels.Success.False, context.ChangeType.ToString()).Observe(sw.ElapsedMilliseconds);
        }

        if (!isSuccess)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var latestPreviousSnapshot = await dbContext.TargetSnapshots
                    .Where(snapshot => snapshot.TargetId == context.Id)
                    .OrderByDescending(snapshot => snapshot.CreatedAt)
                    .FirstOrDefaultAsync();

                var failureSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    TargetId = context.Id,
                    Value = latestPreviousSnapshot is not null ? latestPreviousSnapshot.Value : string.Empty,
                    NewValue = latestPreviousSnapshot is not null ? latestPreviousSnapshot.NewValue : string.Empty,
                    IsChangeDetected = false,
                    Outcome = Outcome.Failure,
                    Message = errorMessage,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(failureSnapshot);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
