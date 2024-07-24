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

namespace WebPageChangeMonitor.Services.Strategies;

public class SnapshotChangeDetectionStrategy : IChangeDetectionStrategy
{
    private readonly ILogger<SnapshotChangeDetectionStrategy> _logger;
    private readonly ChangeMonitorOptions _options;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;

    public SnapshotChangeDetectionStrategy(
        ILogger<SnapshotChangeDetectionStrategy> logger,
        IOptions<ChangeMonitorOptions> options,
        IDbContextFactory<MonitorDbContext> contextFactory)
    {
        _logger = logger;
        _options = options.Value;
        _contextFactory = contextFactory;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Snapshot;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        using (var dbContext = _contextFactory.CreateDbContext())
        {
            var latestSnapshot = await dbContext.TargetSnapshots
                .OrderByDescending(snapshot => snapshot.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestSnapshot is null) 
            {
                var initSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    Value = html,
                    IsChangeDetected = false,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(initSnapshot);
                await dbContext.SaveChangesAsync();

                return;
            }

            var isChangeDetected = latestSnapshot.Value != html;

            var snapshot = new TargetSnapshotEntity()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Value = html,
                IsChangeDetected = isChangeDetected,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.TargetSnapshots.Add(snapshot);
            await dbContext.SaveChangesAsync();

            if (isChangeDetected && _options.AreNotificationsEnabled)
            {
                // todo implement notification mechanism here
                
                _logger.LogInformation(
                    "Expected value detected for url {Url}. Sending notifications out...",
                    context.Url);
            }
        }
    }
}
