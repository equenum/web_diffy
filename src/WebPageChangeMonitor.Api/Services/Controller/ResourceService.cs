using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UUIDNext;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Api.Models.Requests;
using WebPageChangeMonitor.Api.Responses;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

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

    public async Task<ResourcePaginatedResponse> GetAsync(int? page, int count)
    {
        if (page.HasValue && page.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), $"Expected range: {nameof(page)} > 0.");
        }

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Expected range: {nameof(count)} > 0.");
        }

        var availableCount = await _context.Resources.CountAsync();

        // todo only fetch values if available count is more than 0
        var resourceQuery = page.HasValue
            ? _context.Resources.Skip((page.Value - 1) * count).Take(count)
            : _context.Resources;

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

    public async Task<ResourceDto> CreateAsync(CreateRecourseRequest request)
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

        await _jobService.UnscheduleByResourceAsync(id);

        _context.Resources.Remove(targetResource);
        await _context.SaveChangesAsync();
    }
}
