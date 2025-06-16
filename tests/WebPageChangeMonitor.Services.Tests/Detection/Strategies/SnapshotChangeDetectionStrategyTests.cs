using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using UUIDNext;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Detection.Strategies;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Tests.Detection.Strategies;

public class SnapshotChangeDetectionStrategyTests : IDisposable
{
    private readonly string DbName = nameof(SnapshotChangeDetectionStrategyTests);

    private readonly IDbContextFactory<MonitorDbContext> _dbContextFactoryMock;
    private readonly IHtmlParser _parserMock;

    private readonly SnapshotChangeDetectionStrategy _strategy;

    public SnapshotChangeDetectionStrategyTests()
    {
        _dbContextFactoryMock = Substitute.For<IDbContextFactory<MonitorDbContext>>();
        _parserMock = Substitute.For<IHtmlParser>();

        _strategy = new SnapshotChangeDetectionStrategy(
            Substitute.For<ILogger<SnapshotChangeDetectionStrategy>>(),
            Options.Create(GetDefaultOptions()),
            _dbContextFactoryMock,
            _parserMock);
    }

    [Theory]
    [InlineData(ChangeType.Value)]
    public void CanHandle_UnsupportedChangeTypes_ReturnsFalse(ChangeType type) =>
        _strategy.CanHandle(type).Should().BeFalse();

    [Fact]
    public void CanHandle_SupportedChangeType_ReturnsTrue() =>
        _strategy.CanHandle(ChangeType.Snapshot).Should().BeTrue();

    [Fact]
    public async Task ExecuteAsync_FirstEverSnapshot_SavesNewSnapshot()
    {
        // Arrange
        const string newValue = "new";
        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql)
        };

        _dbContextFactoryMock.CreateDbContext().Returns(FakeDbContext.GetInstance(DbName));
        _parserMock.GetNodeInnerText(Arg.Any<string>(), targetContext).Returns(newValue);

        // Act
        await _strategy.ExecuteAsync(string.Empty, targetContext);

        var dbTargetSnapshots = await FakeDbContext.GetInstance(DbName).TargetSnapshots
            .OrderBy(snapshot => snapshot.CreatedAt)
            .ToListAsync();

        // Assert
        dbTargetSnapshots.Should().ContainSingle();
        dbTargetSnapshots[0].Value.Should().Be(newValue);
        dbTargetSnapshots[0].IsChangeDetected.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_NotFirstSnapshotChangesDetected_UpdatesSnapshot()
    {
        // Arrange
        const string newValue = "new";
        const string previousValue = "current";

        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql)
        };

        _dbContextFactoryMock.CreateDbContext().Returns(FakeDbContext.GetInstance(DbName));
        _parserMock.GetNodeInnerText(Arg.Any<string>(), targetContext).Returns(newValue);

        using (var context = FakeDbContext.GetInstance(DbName))
        {
            context.TargetSnapshots.Add(new TargetSnapshotEntity
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                TargetId = targetContext.Id,
                Value = previousValue,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        // Act
        await _strategy.ExecuteAsync("html", targetContext);

        var dbTargetSnapshots = await FakeDbContext.GetInstance(DbName).TargetSnapshots
            .OrderBy(snapshot => snapshot.CreatedAt)
            .ToListAsync();

        // Assert
        dbTargetSnapshots.Should().HaveCount(2);
        dbTargetSnapshots[1].TargetId.Should().Be(dbTargetSnapshots[0].TargetId);
        dbTargetSnapshots[1].Value.Should().Be(newValue);
        dbTargetSnapshots[1].IsChangeDetected.Should().BeTrue();
        dbTargetSnapshots[1].CreatedAt.Should().BeAfter(dbTargetSnapshots[0].CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_NotFirstSnapshotNoChangesDetected_UpdatesSnapshot()
    {
        // Arrange
        const string newValue = "test";
        const string previousValue = "test";

        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql)
        };

        _dbContextFactoryMock.CreateDbContext().Returns(FakeDbContext.GetInstance(DbName));
        _parserMock.GetNodeInnerText(Arg.Any<string>(), targetContext).Returns(newValue);

        using (var context = FakeDbContext.GetInstance(DbName))
        {
            context.TargetSnapshots.Add(new TargetSnapshotEntity
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                TargetId = targetContext.Id,
                Value = previousValue,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        // Act
        await _strategy.ExecuteAsync("html", targetContext);

        var dbTargetSnapshots = await FakeDbContext.GetInstance(DbName).TargetSnapshots
            .OrderBy(snapshot => snapshot.CreatedAt)
            .ToListAsync();

        // Assert
        dbTargetSnapshots.Should().HaveCount(2);
        dbTargetSnapshots[1].TargetId.Should().Be(dbTargetSnapshots[0].TargetId);
        dbTargetSnapshots[1].Value.Should().Be(previousValue);
        dbTargetSnapshots[1].IsChangeDetected.Should().BeFalse();
        dbTargetSnapshots[1].CreatedAt.Should().BeAfter(dbTargetSnapshots[0].CreatedAt);
    }

    public void Dispose()
    {
        FakeDbContext.Reset(DbName);
    }

    private static ChangeMonitorOptions GetDefaultOptions() => new()
    {
        AreNotificationsEnabled = false
    };
}
