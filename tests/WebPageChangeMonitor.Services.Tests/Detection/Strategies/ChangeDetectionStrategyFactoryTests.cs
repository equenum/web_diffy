using System;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Services.Detection.Strategies;

namespace WebPageChangeMonitor.Services.Tests.Detection.Strategies;

public class ChangeDetectionStrategyFactoryTests
{
    private readonly IChangeDetectionStrategy _strategyMock;
    private readonly ChangeDetectionStrategyFactory _factory;

    public ChangeDetectionStrategyFactoryTests()
    {
        _strategyMock = Substitute.For<IChangeDetectionStrategy>();
        _strategyMock.CanHandle(ChangeType.Value).Returns(true);
        
        _factory = new ChangeDetectionStrategyFactory([ _strategyMock ]);
    }

    [Fact]
    public void Get_NoStrategyAssociatedWithChangeType_ThrowsException()
    {
        // Arrange
        const ChangeType changeType = ChangeType.Snapshot;

        // Act
        var action = () => _factory.Get(changeType);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Unexpected change type value: {changeType}");

        _strategyMock.Received(1).CanHandle(changeType);
    }

    [Fact]
    public void Get_StrategyAssociatedWithChangeType_ReturnsExpectedStrategy()
    {
        // Arrange
        const ChangeType changeType = ChangeType.Value;
       
        // Act
        var strategy = _factory.Get(changeType);

        // Assert
        strategy.Should().NotBeNull();
        _strategyMock.Received(1).CanHandle(changeType);
    }
}
