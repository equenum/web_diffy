using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Filters;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Models.Requests;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Endpoints;

public static class ResourceEndpoints
{
    private const string GetByIdEndpointName = "GetResourceById";
    private static readonly Type ResourceEndpointsType = typeof(ResourceEndpoints);

    public static void MapResourceEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/resources");

        group.MapGet(string.Empty, GetAll);
        group.MapGet("{id}", GetById).WithName(GetByIdEndpointName);

        group.MapPost(string.Empty, Create).AddEndpointFilter<CreateResourceValidationFilter>();
        group.MapPut(string.Empty, Update).AddEndpointFilter<UpdateResourceValidationFilter>();;
        group.MapDelete("{id}", Remove);
    }

    public static async Task<Results<Ok<ResourcePaginatedResponse>, BadRequest<string>>> GetAll(
        int? page,
        int? count,
        SortDirection? sortDirection,
        string sortBy,
        ILoggerFactory loggerFactory,
        IOptions<ChangeMonitorOptions> options,
        IResourceService service)
    {
        try
        {
            var response = await service.GetAsync(sortDirection, sortBy, page, count ?? options.Value.DefaultResourcePageSize);
            return TypedResults.Ok(response);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            var logger = loggerFactory.CreateLogger(ResourceEndpointsType);
            logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);

            return TypedResults.BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    public static async Task<Results<Ok<ResourceDto>, NotFound<string>>> GetById(
        Guid id,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            var resource = await service.GetAsync(id);
            return TypedResults.Ok(resource);
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(ResourceEndpointsType);
            logger.LogError("Resource not found: {Id}", id);

            return TypedResults.NotFound($"Resource not found: {id}");
        }
    }

    public static async Task<CreatedAtRoute<ResourceDto>> Create(
        CreateResourceRequest request,
        IResourceService service)
    {
        var resource = await service.CreateAsync(request);

        return TypedResults.CreatedAtRoute(routeName: GetByIdEndpointName,
            routeValues: new { id = resource.Id },
            value: resource);
    }

    public static async Task<Results<CreatedAtRoute<ResourceDto>, NotFound<string>>> Update(
        Resource resource,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            var updatedResource = await service.UpdateAsync(resource);

            return TypedResults.CreatedAtRoute(routeName: GetByIdEndpointName,
                routeValues: new { id = updatedResource.Id },
                value: updatedResource);
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(ResourceEndpointsType);
            logger.LogError("Resource not found: {Id}", resource.Id);

            return TypedResults.NotFound($"Resource not found: {resource.Id}");
        }
    }

    public static async Task<Results<NoContent, NotFound<string>>> Remove(
        Guid id,
        ILoggerFactory loggerFactory,
        IResourceService service)
    {
        try
        {
            await service.RemoveAsync(id);
            return TypedResults.NoContent();
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(ResourceEndpointsType);
            logger.LogError("Resource not found: {Id}", id);

            return TypedResults.NotFound($"Resource not found: {id}");
        }
    }
}
