using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UUIDNext;
using WebPageChangeMonitor.Common.Stats;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Logging;
using WebPageChangeMonitor.Models.Notifications;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Notifications;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Detection.Strategies;

public class ValueChangeDetectionStrategy : IChangeDetectionStrategy
{
    private readonly ILogger<ValueChangeDetectionStrategy> _logger;
    private readonly IHtmlParser _htmlParser;
    private readonly ChangeMonitorOptions _options;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;
    private readonly INotificationService _notificationService;

    public ValueChangeDetectionStrategy(
        ILogger<ValueChangeDetectionStrategy> logger,
        IHtmlParser htmlParser,
        IOptions<ChangeMonitorOptions> options,
        IDbContextFactory<MonitorDbContext> contextFactory,
        INotificationService notificationService)
    {
        _logger = logger;
        _htmlParser = htmlParser;
        _options = options.Value;
        _contextFactory = contextFactory;
        _notificationService = notificationService;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Value;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        using (var dbContext = _contextFactory.CreateDbContext())
        {
            if (context.ExpectedValue is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(context.ExpectedValue)} expected for value based change detection.");
            }

            var newValue = _htmlParser.GetNodeInnerText(html, context);
            var isExpectedValue = context.ExpectedValue == newValue;

            var latestPreviousSnapshot = await dbContext.TargetSnapshots
                .Where(snapshot => snapshot.TargetId == context.Id)
                .OrderByDescending(snapshot => snapshot.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestPreviousSnapshot is null)
            {
                var initSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    TargetId = context.Id,
                    Value = newValue,
                    NewValue = newValue,
                    IsExpectedValue = isExpectedValue,
                    IsChangeDetected = false,
                    Outcome = Outcome.Success,
                    Message = "Initial value snapshot created",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(initSnapshot);
                await dbContext.SaveChangesAsync();

                if (isExpectedValue && _options.Notifications.AreEnabled)
                {
                    var resource = await dbContext.Resources.FirstOrDefaultAsync(resource => resource.Id == context.ResourceId);
                    
                    await SendNotification(resource, context, new NotificationMessageContext()
                    {
                        Message = "Initial value snapshot created",
                        IsChangeDetected = false,
                        Snapshot = initSnapshot
                    });
                }

                return;
            }

            var isChangeDetected = latestPreviousSnapshot.NewValue != newValue;

            var snapshot = new TargetSnapshotEntity()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                TargetId = context.Id,
                Value = latestPreviousSnapshot.NewValue,
                NewValue = newValue,
                IsExpectedValue = isExpectedValue,
                IsChangeDetected = isChangeDetected,
                Outcome = Outcome.Success,
                Message = "Consecutive value snapshot created",
                CreatedAt = DateTime.UtcNow
            };

            dbContext.TargetSnapshots.Add(snapshot);
            await dbContext.SaveChangesAsync();

            if (isChangeDetected && isExpectedValue && _options.Notifications.AreEnabled)
            {
                var resource = await dbContext.Resources.FirstOrDefaultAsync(resource => resource.Id == context.ResourceId);
                
                await SendNotification(resource, context, new NotificationMessageContext()
                {
                    Message = "Consecutive value snapshot created",
                    IsChangeDetected = isChangeDetected,
                    Snapshot = snapshot
                });
            }
        }
    }

    private async Task SendNotification(ResourceEntity resource, TargetContext targetContext,
        NotificationMessageContext messageContext)
    { 
        var enabledChannels = _options.Notifications.Channels.Where(channel => channel.IsEnabled is true);
        
        if (resource is not null && enabledChannels.Any())
        {
            foreach (var channel in enabledChannels)
            {
                _logger.LogInformation("Expected value detected for target '{TargetName}' ({TargetId}). Sending notification to {Channel}",
                    targetContext.DisplayName,
                    targetContext.Id,
                    channel.Name);

                var sw = Stopwatch.StartNew();
                
                try
                {
                    var body = new Dictionary<string, string>()
                    {
                        { "resource", resource.DisplayName },
                        { "target", targetContext.DisplayName },
                        { "message", messageContext.Message },
                        { "is-change-detected", messageContext.IsChangeDetected.ToString() }
                    };

                    await _notificationService.SendAsync(channel, new NotificationMessage()
                    {
                        Title = "Expected value detected",
                        Body = JsonSerializer.Serialize(body),
                        BodyType = NotificationBodyType.KeyValue,
                        Origin = _options.Notifications.OriginName,
                        Timestamp = messageContext.Snapshot.CreatedAt
                    });

                    sw.Stop();
                    MonitorMetrics.NotificationSendCount.WithLabels(MetricLabels.Success.True, targetContext.ChangeType.ToString(), channel.Name).Inc();
                    MonitorMetrics.NotificationSendDuration.WithLabels(MetricLabels.Success.True, targetContext.ChangeType.ToString(), channel.Name).Observe(sw.ElapsedMilliseconds);
                }
                catch
                {
                    sw.Stop();

                    _logger.LogError("Err-{ErrorCode}: Failed to send notification to '{Channel}', target id '{TargetId}', target name {TargetName}.",
                        LogErrorCodes.Notifications.ValueFailed,
                        channel.Name,
                        targetContext.Id,
                        targetContext.DisplayName);

                    MonitorMetrics.NotificationSendCount.WithLabels(MetricLabels.Success.False, targetContext.ChangeType.ToString(), channel.Name).Inc();
                    MonitorMetrics.NotificationSendDuration.WithLabels(MetricLabels.Success.False, targetContext.ChangeType.ToString(), channel.Name).Observe(sw.ElapsedMilliseconds);
                }
            }
        }
    }
}
