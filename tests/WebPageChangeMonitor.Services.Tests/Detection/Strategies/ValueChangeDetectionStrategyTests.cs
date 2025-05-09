using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Detection.Strategies;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Tests.Detection.Strategies;

public class ValueChangeDetectionStrategyTests
{
    private readonly IHtmlParser _htmlParserMock;
    private readonly ChangeMonitorOptions _options;
    private readonly MonitorDbContext _dbContextMock;

    private readonly ValueChangeDetectionStrategy _strategy;

    public ValueChangeDetectionStrategyTests()
    {
        _htmlParserMock = Substitute.For<IHtmlParser>();
        _options = new ChangeMonitorOptions();

        var dbOptions = new DbContextOptionsBuilder<MonitorDbContext>()
            .UseInMemoryDatabase("fakeDb")
            .Options;

        _dbContextMock = Substitute.For<MonitorDbContext>(dbOptions);

        var contextFactoryMock = Substitute.For<IDbContextFactory<MonitorDbContext>>();
        contextFactoryMock.CreateDbContext().Returns(_dbContextMock);
        
        _strategy = new ValueChangeDetectionStrategy(NullLogger<ValueChangeDetectionStrategy>.Instance, 
            _htmlParserMock, Options.Create(_options), contextFactoryMock);
    }

    // compose tests
}
