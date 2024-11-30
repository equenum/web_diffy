using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UUIDNext;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Strategies;

public class ValueChangeDetectionStrategy : IChangeDetectionStrategy
{
    private readonly ILogger<ValueChangeDetectionStrategy> _logger;
    private readonly IHtmlParser _htmlParser;
    private readonly ChangeMonitorOptions _options;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;

    public ValueChangeDetectionStrategy(
        ILogger<ValueChangeDetectionStrategy> logger,
        IHtmlParser htmlParser,
        IOptions<ChangeMonitorOptions> options,
        IDbContextFactory<MonitorDbContext> contextFactory)
    {
        _logger = logger;
        _htmlParser = htmlParser;
        _options = options.Value;
        _contextFactory = contextFactory;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Value;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        using (var dbContext = _contextFactory.CreateDbContext())
        {
            if  (context.ExpectedValue is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(context.ExpectedValue)} expected for value based change detection.");
            }

            var currentValue = _htmlParser.GetNodeInnerText(html, context);
            var isExpectedValue = context.ExpectedValue == currentValue;

            var latestSnapshot = await dbContext.TargetSnapshots
                .OrderByDescending(snapshot => snapshot.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestSnapshot is null) 
            {   
                var initSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    TargetId = context.Id,
                    Value = currentValue,
                    IsExpectedValue = isExpectedValue,
                    IsChangeDetected = false,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(initSnapshot);
                await dbContext.SaveChangesAsync();

                if (isExpectedValue && _options.AreNotificationsEnabled)
                {
                    // todo implement notification mechanism here

                    _logger.LogInformation(
                        "Expected value detected for url {Url}. Sending notifications out...",
                        context.Url);
                }

                return;
            }

            var isChangeDetected = latestSnapshot.Value != currentValue;

            var snapshot = new TargetSnapshotEntity()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                TargetId = context.Id,
                Value = currentValue,
                IsExpectedValue = isExpectedValue,
                IsChangeDetected = isChangeDetected,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.TargetSnapshots.Add(snapshot);
            await dbContext.SaveChangesAsync();

            if (isChangeDetected && isExpectedValue && _options.AreNotificationsEnabled)
            {
                // todo implement notification mechanism here

                _logger.LogInformation(
                    "Expected value detected for url {Url}. Sending notifications out...",
                    context.Url);
            }
        }
    }
}
