using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebPageChangeMonitor.Api.Infrastructure;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureEndpointServices();
builder.Services.ConfigureHostedServices();

var connectionString = builder.Configuration.GetConnectionString("ChangeMonitor");

builder.Services.AddJobManagementServices();
builder.Services.AddChangeDetectionServices();
builder.Services.AddDataAccessServices(connectionString);
builder.Services.AddEndpointServices();
builder.Services.AddOtherServices();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapResourceEndpoints();
app.MapTargetEndpoints();
app.MapTargetSnapshotEndpoints();

await DbInitializer.ExecuteAsync(connectionString);

app.Run();
