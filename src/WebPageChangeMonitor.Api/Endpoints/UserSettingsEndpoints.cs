using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Logging;

namespace WebPageChangeMonitor.Api.Endpoints;

public static class UserSettingsEndpoints
{
    private const string GetEndpointName = "GetUserSettings";
    private static readonly Type UserSettingsEndpointsType = typeof(UserSettingsEndpoints);

    public static void MapUserSettingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/user-settings");

        group.MapGet(string.Empty, Get).WithName(GetEndpointName); ;
        group.MapPost(string.Empty, Create);
        group.MapPut(string.Empty, Update);
    }

    public static async Task<Results<Ok<UserSettingsDto>, NotFound<string>>> Get(
        ILoggerFactory loggerFactory,
        IUserSettingsService service)
    {
        try
        {
            var settings = await service.GetAsync();
            return TypedResults.Ok(settings);
        }
        catch (UserSettingsNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(UserSettingsEndpointsType);
            logger.LogError("Err-{ErrorCode}: User settings not found.", LogErrorCodes.UserSettings.NotFound);

            return TypedResults.NotFound("User settings not found.");
        }
    }

    public static async Task<CreatedAtRoute<UserSettingsDto>> Create(
        UserSettingsDto settings,
        IUserSettingsService service)
    {
        var updatedSettings = await service.CreateAsync(settings);

        return TypedResults.CreatedAtRoute(routeName: GetEndpointName,
            value: updatedSettings);
    }
    
    public static async Task<Results<CreatedAtRoute<UserSettingsDto>, NotFound<string>>> Update(
        UserSettingsDto settings,
        ILoggerFactory loggerFactory,
        IUserSettingsService service)
    {
        try
        {
            var updatedSettings = await service.UpdateAsync(settings);

            return TypedResults.CreatedAtRoute(routeName: GetEndpointName,
                value: updatedSettings);
        }
        catch (UserSettingsNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(UserSettingsEndpointsType);
            logger.LogError("Err-{ErrorCode}: User settings not found.", LogErrorCodes.UserSettings.NotFound);

            return TypedResults.NotFound("User settings not found.");
        }
    }
}
