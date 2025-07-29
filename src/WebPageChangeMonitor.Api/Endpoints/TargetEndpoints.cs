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

public static class TargetEndpoints
{
    private const string GetByIdEndpointName = "GetTargetById";
    private static readonly Type TargetEndpointsType = typeof(TargetEndpoints);

    public static void MapTargetEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/targets");

        group.MapGet(string.Empty, GetAll);
        group.MapGet("{id}", GetById).WithName(GetByIdEndpointName);
        group.MapGet("resource/{id}", GetByResourceIdAsync);

        group.MapPost(string.Empty, Create).AddEndpointFilter<CreateTargetValidationFilter>();
        group.MapPut(string.Empty, Update).AddEndpointFilter<UpdateTargetValidationFilter>();

        group.MapDelete("{id}", Remove);
        group.MapDelete("resource/{id}", RemoveByResourceId);
    }

    public static async Task<Results<Ok<TargetPaginatedResponse>, BadRequest<string>>> GetAll(
        int? page,
        int? count,
        SortDirection? sortDirection,
        string sortBy,
        ILoggerFactory loggerFactory,
        IOptions<ChangeMonitorOptions> options,
        ITargetService service)
    {
        try
        {
            var response = await service.GetAsync(sortDirection, sortBy, page, count ?? options.Value.DefaultTargetPageSize);
            return TypedResults.Ok(response);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);

            return TypedResults.BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    public static async Task<Results<Ok<TargetDto>, NotFound<string>>> GetById(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetService service)
    {
        try
        {
            var target = await service.GetAsync(id);
            return TypedResults.Ok(target);
        }
        catch (TargetNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Target not found: {Id}", id);

            return TypedResults.NotFound($"Target not found: {id}");
        }
    }

    public static async Task<Results<Ok<TargetPaginatedResponse>, BadRequest<string>>> GetByResourceIdAsync(
        Guid id,
        int? page,
        int? count,
        SortDirection? sortDirection,
        string sortBy,
        ILoggerFactory loggerFactory,
        IOptions<ChangeMonitorOptions> options,
        ITargetService service)
    {
        try
        {
            var response = await service.GetByResourceIdAsync(id, sortDirection, sortBy, page, count ?? options.Value.DefaultTargetPageSize);
            return TypedResults.Ok(response);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);

            return TypedResults.BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    public static async Task<CreatedAtRoute<TargetDto>> Create(
        CreateTargetRequest request,
        ITargetService service)
    {
        var target = await service.CreateAsync(request);

        return TypedResults.CreatedAtRoute(routeName: GetByIdEndpointName,
            routeValues: new { id = target.Id },
            value: target);
    }

    public static async Task<Results<CreatedAtRoute<TargetDto>, BadRequest<string>, NotFound<string>>> Update(
        Target target,
        ILoggerFactory loggerFactory,
        ITargetService service)
    {
        try
        {
            var updatedTarget = await service.UpdateAsync(target);

            return TypedResults.CreatedAtRoute(routeName: GetByIdEndpointName,
                routeValues: new { id = updatedTarget.Id },
                value: updatedTarget);
        }
        catch (ResourceNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Resource doesn't exist: {ResourceId}", target.ResourceId);

            return TypedResults.BadRequest($"Resource doesn't exist: {target.ResourceId}");
        }
        catch (TargetNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Target not found: {TargetId}", target.Id);

            return TypedResults.NotFound($"Target not found: {target.Id}");
        }
    }

    public static async Task<Results<NoContent, NotFound<string>>> Remove(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetService service)
    {
        try
        {
            await service.RemoveAsync(id);
            return TypedResults.NoContent();
        }
        catch (TargetNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Target not found: {Id}", id);

            return TypedResults.NotFound($"Target not found: {id}");
        }
    }

    public static async Task<Results<NoContent, NotFound<string>>> RemoveByResourceId(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetService service)
    {
        try
        {
            await service.RemoveByResourceIdAsync(id);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            var logger = loggerFactory.CreateLogger(TargetEndpointsType);
            logger.LogError("Failed to remove targets: {Message}.", ex.Message);

            return TypedResults.NotFound(ex.Message);
        }
    }
}
