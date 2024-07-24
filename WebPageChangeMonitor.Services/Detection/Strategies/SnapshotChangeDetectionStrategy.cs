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
    private readonly MonitorDbContext _context;
    private readonly ChangeMonitorOptions _options;

    public SnapshotChangeDetectionStrategy(
        ILogger<SnapshotChangeDetectionStrategy> logger,
        MonitorDbContext context,
        IOptions<ChangeMonitorOptions> options)
    {
        _logger = logger;
        _context = context;
        _options = options.Value;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Snapshot;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        var latestSnapshot = await _context.TargetSnapshots
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

            _context.TargetSnapshots.Add(initSnapshot);
            await _context.SaveChangesAsync();

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

        _context.TargetSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        if (isChangeDetected && _options.AreNotificationsEnabled)
        {
            // todo implement notification mechanism here
            
            _logger.LogInformation(
                    "Expected value detected for url {Url}. Sending notifications out...",
                    context.Url);
        }
    }
}
