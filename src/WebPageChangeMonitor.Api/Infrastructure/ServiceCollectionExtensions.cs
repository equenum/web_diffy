using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using WebPageChangeMonitor.Api.Infrastructure.Schemas;
using WebPageChangeMonitor.Api.Services;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Services.Detection.Strategies;
using WebPageChangeMonitor.Services.Notifications;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureEndpointServices(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<EnumSchemaFilter>();
        });

        return services;
    }

    public static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<MonitorJobsRegistrationService>();

        services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

        services.AddQuartz();
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = false;
        });

        return services;
    }

    public static IServiceCollection AddJobManagementServices(this IServiceCollection services)
    {
        services.AddTransient<MonitorChangeJob>();
        services.AddSingleton<IJobFactory, MonitorJobFactory>();
        services.AddTransient<IMonitorJobService, MonitorJobService>();

        return services;
    }

    public static IServiceCollection AddChangeDetectionServices(this IServiceCollection services)
    {
        services.AddTransient<IChangeDetector, ChangeDetector>();
        services.AddTransient<IChangeDetectionStrategyFactory, ChangeDetectionStrategyFactory>();
        services.AddTransient<IChangeDetectionStrategy, ValueChangeDetectionStrategy>();
        services.AddTransient<IChangeDetectionStrategy, SnapshotChangeDetectionStrategy>();
        services.AddTransient<INotificationService, NotificationService>();

        return services;
    }

    public static IServiceCollection AddDataAccessServices(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextFactory<MonitorDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        return services;
    }

    public static IServiceCollection AddEndpointServices(this IServiceCollection services)
    {
        services.AddTransient<IResourceService, ResourceService>();
        services.AddTransient<ITargetService, TargetService>();
        services.AddTransient<ITargetSnapshotService, TargetSnapshotService>();
        services.AddTransient<IUserSettingsService, UserSettingsService>();

        return services;
    }

    public static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        services.AddTransient<IHtmlParser, HtmlParser>();

        return services;
    }
    
    public static IServiceCollection ConfigureOptions(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<ChangeMonitorOptions>(configuration.GetSection(ChangeMonitorOptions.SectionName));

        return services;
    }
}
