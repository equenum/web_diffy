using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Models.Options;

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
