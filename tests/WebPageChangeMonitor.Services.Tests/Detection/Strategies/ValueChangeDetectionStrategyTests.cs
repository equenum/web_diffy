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
using WebPageChangeMonitor.Services.Notifications;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Tests.Detection.Strategies;

public class ValueChangeDetectionStrategyTests : IDisposable
{
    private readonly string DbName = nameof(ValueChangeDetectionStrategyTests);

    private readonly IDbContextFactory<MonitorDbContext> _dbContextFactoryMock;
    private readonly IHtmlParser _parserMock;
    private readonly INotificationService _notificationServiceMock;

    private readonly ValueChangeDetectionStrategy _strategy;

    public ValueChangeDetectionStrategyTests()
    {
        _dbContextFactoryMock = Substitute.For<IDbContextFactory<MonitorDbContext>>();
        _parserMock = Substitute.For<IHtmlParser>();

        _strategy = new ValueChangeDetectionStrategy(
            Substitute.For<ILogger<ValueChangeDetectionStrategy>>(),
            _parserMock,
            Options.Create(GetDefaultOptions()),
            _dbContextFactoryMock,
            _notificationServiceMock);
    }

    [Theory]
    [InlineData(ChangeType.Snapshot)]
    public void CanHandle_UnsupportedChangeTypes_ReturnsFalse(ChangeType type) =>
        _strategy.CanHandle(type).Should().BeFalse();

    [Fact]
    public void CanHandle_SupportedChangeType_ReturnsTrue() =>
        _strategy.CanHandle(ChangeType.Value).Should().BeTrue();

    [Fact]
    public async Task ExecuteAsync_NullExpectedValue_ThrowsException()
    {
        // Arrange
        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql)
        };

        _dbContextFactoryMock.CreateDbContext().Returns(FakeDbContext.GetInstance(DbName));

        // Act
        var action = () => _strategy.ExecuteAsync(string.Empty, targetContext);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"{nameof(TargetContext.ExpectedValue)} expected for value based change detection.");
    }

    [Theory]
    [InlineData("new", true)]
    [InlineData("test", false)]
    public async Task ExecuteAsync_FirstEverSnapshot_SavesNewSnapshot(string expectedValue,
        bool expectedIsExpectedValue)
    {
        // Arrange
        const string newValue = "new";

        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            ExpectedValue = expectedValue
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
        dbTargetSnapshots[0].NewValue.Should().Be(newValue);
        dbTargetSnapshots[0].IsChangeDetected.Should().BeFalse();
        dbTargetSnapshots[0].IsExpectedValue.Should().Be(expectedIsExpectedValue);
        dbTargetSnapshots[0].Outcome.Should().Be(Outcome.Success);
        dbTargetSnapshots[0].Message.Should().Be("Initial value snapshot created");
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("other", false)]
    public async Task ExecuteAsync_NotFirstSnapshotNoChangesDetected_UpdatesSnapshot(
        string expectedValue, bool expectedIsExpectedValue
    )
    {
        // Arrange
        const string newValue = "test";
        const string previousValue = "test";

        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            ExpectedValue = expectedValue
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
                NewValue = previousValue,
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
        dbTargetSnapshots[1].NewValue.Should().Be(newValue);
        dbTargetSnapshots[1].IsChangeDetected.Should().BeFalse();
        dbTargetSnapshots[1].CreatedAt.Should().BeAfter(dbTargetSnapshots[0].CreatedAt);
        dbTargetSnapshots[1].IsExpectedValue.Should().Be(expectedIsExpectedValue);
        dbTargetSnapshots[1].Outcome.Should().Be(Outcome.Success);
        dbTargetSnapshots[1].Message.Should().Be("Consecutive value snapshot created");
    }

    [Theory]
    [InlineData("new", true)]
    [InlineData("current", false)]
    public async Task ExecuteAsync_NotFirstSnapshotChangesDetected_UpdatesSnapshot(
        string expectedValue, bool expectedIsExpectedValue)
    {
        // Arrange
        const string newValue = "new";
        const string previousValue = "current";

        var targetContext = new TargetContext()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            ExpectedValue = expectedValue
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
                NewValue = previousValue,
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
        dbTargetSnapshots[1].NewValue.Should().Be(newValue);
        dbTargetSnapshots[1].IsChangeDetected.Should().BeTrue();
        dbTargetSnapshots[1].CreatedAt.Should().BeAfter(dbTargetSnapshots[0].CreatedAt);
        dbTargetSnapshots[1].IsExpectedValue.Should().Be(expectedIsExpectedValue);
        dbTargetSnapshots[1].Outcome.Should().Be(Outcome.Success);
        dbTargetSnapshots[1].Message.Should().Be("Consecutive value snapshot created");
    }

    public void Dispose()
    {
        FakeDbContext.Reset(DbName);
    }

    private static ChangeMonitorOptions GetDefaultOptions() => new()
    {
        Notifications = new NotificationOptions
        {
            AreEnabled = false
        }
    };
}
