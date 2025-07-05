using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Models.Requests;

namespace WebPageChangeMonitor.Api.Endpoints;

public static class ResourceEndpoints
{
    private const string GetByIdEndpointName = "GetById";

    public static void MapResourceEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/resources");

        group.MapGet(string.Empty, GetAll);
        group.MapGet("{id}", GetById).WithName(GetByIdEndpointName);
        group.MapPost(string.Empty, Create);
        group.MapPut(string.Empty, Update);
        group.MapDelete("{id}", Remove);
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

    public static async Task<IResult> GetById(
        Guid id,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            var resource = await service.GetAsync(id);
            return Results.Ok(resource);
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(typeof(ResourceEndpoints));
            logger.LogError("Resource not found: {Id}", id);

            return Results.NotFound($"Resource not found: {id}");
        }
    }

    public static async Task<IResult> Create(
        CreateResourceRequest request,
        IResourceService service)
    {
        var resource = await service.CreateAsync(request);

        return Results.CreatedAtRoute(routeName: GetByIdEndpointName,
            routeValues: new { id = resource.Id },
            value: resource);
    }

    public static async Task<IResult> Update(
        Resource resource,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            var updatedResource = await service.UpdateAsync(resource);

            return Results.CreatedAtRoute(routeName: GetByIdEndpointName,
                routeValues: new { id = updatedResource.Id },
                value: updatedResource);
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(typeof(ResourceEndpoints));
            logger.LogError("Resource not found: {Id}", resource.Id);

            return Results.NotFound($"Resource not found: {resource.Id}");
        }
    }

    public static async Task<IResult> Remove(
        Guid id,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            await service.RemoveAsync(id);
            return Results.NoContent();
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(typeof(ResourceEndpoints));
            logger.LogError("Resource not found: {Id}", id);

            return Results.NotFound($"Resource not found: {id}");
        }
    }
}
