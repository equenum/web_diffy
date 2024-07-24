using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Api.Infrastructure.Schemas;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Parsers;
using WebPageChangeMonitor.Services.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => 
{
    options.SchemaFilter<EnumSchemaFilter>();
});

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

// job management
builder.Services.AddTransient<MonitorChangeJob>();   
builder.Services.AddSingleton<IJobFactory, MonitorJobFactory>();
builder.Services.AddTransient<IMonitorJobService, MonitorJobService>();

// change detection
builder.Services.AddTransient<IChangeDetector, ChangeDetector>();
builder.Services.AddTransient<IChangeDetectionStrategyFactory, ChangeDetectionStrategyFactory>();
builder.Services.AddTransient<IChangeDetectionStrategy, ValueChangeDetectionStrategy>();
builder.Services.AddTransient<IChangeDetectionStrategy, SnapshotChangeDetectionStrategy>();

// data access
builder.Services.AddDbContextFactory<MonitorDbContext>(options => 
{
    options.UseLazyLoadingProxies();
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChangeMonitor"));
    options.UseSnakeCaseNamingConvention();
});

// worker services
builder.Services.AddTransient<IResourceService, ResourceService>();
builder.Services.AddTransient<ITargetService, TargetService>();
builder.Services.AddTransient<ITargetSnapshotService, TargetSnapshotService>();

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
app.MapControllers();

app.Run();
