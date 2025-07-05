using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api.Endpoints;

public static class ResourceEndpoints
{
    public static void MapResourceEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/resources");

        group.MapGet(string.Empty, GetAll);
    }


    public static async Task<IResult> GetAll(
        int? page,
        int? count,
        ILoggerFactory loggerFactory,
        IOptions<ChangeMonitorOptions> options,
        IResourceService service)
    {
        try
        {
            var response = await service.GetAsync(page, count ?? options.Value.DefaultResourcePageSize);
            return Results.Ok(response);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            var logger = loggerFactory.CreateLogger(typeof(ResourceEndpoints));
            logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);

            return Results.BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }
}
