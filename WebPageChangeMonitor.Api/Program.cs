using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Parsers;

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
builder.Services.AddTransient<MonitorChangeJob>();   
builder.Services.AddSingleton<IJobFactory, MonitorJobFactory>();
builder.Services.AddTransient<IHtmlParser, HtmlParser>();
builder.Services.AddTransient<IChangeDetector, ChangeDetector>();

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
