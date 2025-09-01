using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Logging;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Endpoints;

public static class TargetSnapshotEndpoints
{
    private const string GetByIdEndpointName = "GetTargetSnapshotById";
    private static readonly Type TargetSnapshotEndpointsType = typeof(TargetSnapshotEndpoints);

    public static void MapTargetSnapshotEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/public/snapshots");

        group.MapGet("{id}", GetById).WithName(GetByIdEndpointName);
        group.MapGet("target/{id}", GetByTargetId);

        group.MapDelete("{id}", Remove);
        group.MapDelete("target/{id}", RemoveByTargetId);
    }

    public static async Task<Results<Ok<TargetSnapshotDto>, NotFound<string>>> GetById(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetSnapshotService service)
    {
        try
        {
            var targetSnapshot = await service.GetAsync(id);
            return TypedResults.Ok(targetSnapshot);
        }
        catch (TargetSnapshotNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetSnapshotEndpointsType);
            logger.LogError("Err-{ErrorCode}: Target snapshot not found: {Id}", LogErrorCodes.Snapshot.NotFound, id);

            return TypedResults.NotFound($"Target snapshot not found: {id}");
        }
    }

    public static async Task<Results<Ok<TargetSnapshotPaginatedResponse>, BadRequest<string>>> GetByTargetId(
        Guid id,
        int? page,
        int? count,
        SortDirection? sortDirection,
        string sortBy,
        ILoggerFactory loggerFactory,
        IOptions<ChangeMonitorOptions> options,
        ITargetSnapshotService service)
    {
        try
        {
            var response = await service.GetByTargetIdAsync(id, sortDirection, sortBy, page, count ?? options.Value.DefaultTargetSnapshotPageSize);
            return TypedResults.Ok(response);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            var logger = loggerFactory.CreateLogger(TargetSnapshotEndpointsType);
            logger.LogError("Err-{ErrorCode}: Invalid query parameter value: {ErrorMessage}.",
                LogErrorCodes.Snapshot.InvalidQuery,
                ex.Message);

            return TypedResults.BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    public static async Task<Results<NoContent, NotFound<string>>> Remove(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetSnapshotService service)
    {
        try
        {
            await service.RemoveAsync(id);
            return TypedResults.NoContent();
        }
        catch (TargetSnapshotNotFoundException)
        {
            var logger = loggerFactory.CreateLogger(TargetSnapshotEndpointsType);
            logger.LogError("Err-{ErrorCode}: Target snapshot not found: {Id}", LogErrorCodes.Snapshot.NotFound, id);

            return TypedResults.NotFound($"Target snapshot not found: {id}");
        }
    }

    public static async Task<Results<NoContent, NotFound<string>>> RemoveByTargetId(
        Guid id,
        ILoggerFactory loggerFactory,
        ITargetSnapshotService service)
    {
        try
        {
            await service.RemoveByTargetIdAsync(id);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            var logger = loggerFactory.CreateLogger(TargetSnapshotEndpointsType);
            logger.LogError("Err-{ErrorCode}: Failed to remove target snapshots: {Message}.",
                LogErrorCodes.Snapshot.DeleteFailed,
                ex.Message);

            return TypedResults.NotFound(ex.Message);
        }
    }
}
