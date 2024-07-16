using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Detection;
using WebPageChangeMonitor.Services.Parsers;
using WebPageChangeMonitor.Services.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ChangeMonitorOptions>(
    builder.Configuration.GetSection(ChangeMonitorOptions.SectionName));

builder.Services.AddHostedService<MonitorJobsRegistrationService>();
builder.Services.Configure<HostOptions>(options => 
{
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(options => 
{
    options.WaitForJobsToComplete = false;
});

// todo extract
// job management
builder.Services.AddTransient<MonitorChangeJob>();   
builder.Services.AddSingleton<IJobFactory, MonitorJobFactory>();

// change detection
builder.Services.AddTransient<IChangeDetector, ChangeDetector>();
builder.Services.AddTransient<IChangeDetectionService, ChangeDetectionService>();
builder.Services.AddTransient<IChangeDetectionStrategyFactory, ChangeDetectionStrategyFactory>();
builder.Services.AddTransient<IChangeDetectionStrategy, ValueChangeDetectionStrategy>();
builder.Services.AddTransient<IChangeDetectionStrategy, SnapshotChangeDetectionStrategy>();

// data access
builder.Services.AddDbContext<MonitorDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChangeMonitor"));
    options.UseSnakeCaseNamingConvention();
});

// other services
builder.Services.AddTransient<IHtmlParser, HtmlParser>();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
