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
    private readonly MonitorDbContext _context;
    private readonly ChangeMonitorOptions _options;

    public ValueChangeDetectionStrategy(
        ILogger<ValueChangeDetectionStrategy> logger,
        IHtmlParser htmlParser,
        MonitorDbContext context,
        IOptions<ChangeMonitorOptions> options)
    {
        _logger = logger;
        _htmlParser = htmlParser;
        _context = context;
        _options = options.Value;
    }

    public bool CanHandle(ChangeType type) => type == ChangeType.Value;

    public async Task ExecuteAsync(string html, TargetContext context)
    {
        if  (context.ExpectedValue is null)
        {
            throw new InvalidOperationException(
                $"{nameof(context.ExpectedValue)} expected for value based change detection.");
        }

        var currentValue = _htmlParser.GetNodeInnerText(html, context);
        var isExpectedValue = context.ExpectedValue == currentValue;

        var latestSnapshot = await _context.TargetSnapshots
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .FirstOrDefaultAsync();

        if (latestSnapshot is null) 
        {   
            var initSnapshot = new TargetSnapshotEntity()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Value = currentValue,
                IsExpectedValue = isExpectedValue,
                IsChangeDetected = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.TargetSnapshots.Add(initSnapshot);
            await _context.SaveChangesAsync();

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
            Value = currentValue,
            IsExpectedValue = isExpectedValue,
            IsChangeDetected = isChangeDetected,
            CreatedAt = DateTime.UtcNow
        };

        _context.TargetSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        if (isChangeDetected && isExpectedValue && _options.AreNotificationsEnabled)
        {
            // todo implement notification mechanism here
            
            _logger.LogInformation(
                    "Expected value detected for url {Url}. Sending notifications out...",
                    context.Url);
        }
    }
}
