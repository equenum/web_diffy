﻿using System;
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

namespace WebPageChangeMonitor.Services.Detection.Strategies;

public class SnapshotChangeDetectionStrategy : IChangeDetectionStrategy
{
    private readonly ILogger<SnapshotChangeDetectionStrategy> _logger;
    private readonly ChangeMonitorOptions _options;
    private readonly IDbContextFactory<MonitorDbContext> _contextFactory;
    private readonly IHtmlParser _htmlParser;

    public SnapshotChangeDetectionStrategy(
        ILogger<SnapshotChangeDetectionStrategy> logger,
        IOptions<ChangeMonitorOptions> options,
        IDbContextFactory<MonitorDbContext> contextFactory,
        IHtmlParser htmlParser)
    {
        _logger = logger;
        _options = options.Value;
        _contextFactory = contextFactory;
        _htmlParser = htmlParser;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Snapshot;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        using (var dbContext = _contextFactory.CreateDbContext())
        {
            var newValue = _htmlParser.GetNodeInnerText(html, context);

            var latestPreviousSnapshot = await dbContext.TargetSnapshots
                .Where(snapshot => snapshot.TargetId == context.Id)
                .OrderByDescending(snapshot => snapshot.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestPreviousSnapshot is null) 
            {
                var initialSnapshot = new TargetSnapshotEntity()
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    TargetId = context.Id,
                    Value = newValue,
                    NewValue = newValue,
                    IsChangeDetected = false,
                    Outcome = Outcome.Success,
                    Message = "Initial snapshot created",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.TargetSnapshots.Add(initialSnapshot);
                await dbContext.SaveChangesAsync();

                return;
            }

            var isChangeDetected = latestPreviousSnapshot.NewValue != newValue;

            var snapshot = new TargetSnapshotEntity()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                TargetId = context.Id,
                Value = latestPreviousSnapshot.NewValue,
                NewValue = newValue,
                IsChangeDetected = isChangeDetected,
                Outcome = Outcome.Success,
                Message = "Consecutive snapshot created",
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
