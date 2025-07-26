using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UUIDNext;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Common.Helpers;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Requests;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Services.Controller;

public class ResourceService : IResourceService
{
    private readonly MonitorDbContext _context;
    private readonly IMonitorJobService _jobService;

    public ResourceService(
        MonitorDbContext context,
        IMonitorJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<ResourcePaginatedResponse> GetAsync(SortDirection? sortDirection, string sortBy, int? page, int count)
    {
        if (sortDirection.HasValue)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                throw new ArgumentException("Value cannot be null", nameof(sortBy));
            }

            if (typeof(ResourceEntity).GetProperty(sortBy) is null)
            {
                throw new ArgumentException("Unexpected sort property value", nameof(sortBy));
            }
        }

        if (page.HasValue && page.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), $"Expected range: {nameof(page)} > 0.");
        }

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Expected range: {nameof(count)} > 0.");
        }

        var availableCount = await _context.Resources.CountAsync();
        if (availableCount == 0)
        {
            return new ResourcePaginatedResponse()
            {
                Resources = [],
                AvailableCount = availableCount
            };
        }

        IQueryable<ResourceEntity> resourceQuery = _context.Resources;

        if (sortDirection.HasValue)
        {
            var propertyLambda = ExpressionHelper.GetPropertyLambda<ResourceEntity>(sortBy);

            resourceQuery = sortDirection switch
            {
                SortDirection.Asc => resourceQuery.OrderBy(propertyLambda),
                SortDirection.Desc => resourceQuery.OrderByDescending(propertyLambda),
                _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), "Unexpected sort direction value."),
            };
        }

        resourceQuery = page.HasValue
            ? resourceQuery.Skip((page.Value - 1) * count).Take(count)
            : resourceQuery.Take(count);

        var resources = await resourceQuery.AsNoTracking().ToListAsync();

        return new ResourcePaginatedResponse()
        {
            Resources = resources.Select(resource => resource.ToResourceDto()),
            AvailableCount = availableCount
        };
    }

    public async Task<ResourceDto> GetAsync(Guid id)
    {
        var targetResource = await _context.Resources.FindAsync(id);
        if (targetResource is null)
        {
            throw new ResourceNotFoundException(id.ToString());
        }

        return targetResource.ToResourceDto();
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceRequest request)
    {
        var resource = new ResourceEntity()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            DisplayName = request.DisplayName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        return resource.ToResourceDto();
    }

    public async Task<ResourceDto> UpdateAsync(Resource updatedResource)
    {
        var targetResource = await _context.Resources.FindAsync(updatedResource.Id);
        if (targetResource is null)
        {
            throw new ResourceNotFoundException(updatedResource.ToString());
        }

        _context.Entry(targetResource).CurrentValues.SetValues(updatedResource.ToResourceEntity());
        await _context.SaveChangesAsync();

        return targetResource.ToResourceDto();
    }

    public async Task RemoveAsync(Guid id)
    {
        var targetResource = await _context.Resources.FindAsync(id);
        if (targetResource is null)
        {
            throw new ResourceNotFoundException(id.ToString());
        }

        _context.Resources.Remove(targetResource);
        await _context.SaveChangesAsync();
        
        await _jobService.UnscheduleByResourceAsync(id);
    }
}
