using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UUIDNext;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Api.Models.Requests;
using WebPageChangeMonitor.Api.Models.Responses;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Api.Services.Controller;

public class TargetService : ITargetService
{
    private readonly MonitorDbContext _context;
    private readonly IMonitorJobService _jobService;

    public TargetService(
        MonitorDbContext context,
        IMonitorJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<TargetPaginatedResponse> GetAsync(int? page, int count)
    {
        if (page.HasValue && page.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), $"Expected range: {nameof(page)} > 0.");
        }

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Expected range: {nameof(count)} > 0.");
        }

        var availableCount = await _context.Targets.CountAsync();

        // todo only fetch values if available count is more than 0
        var targetQuery = page.HasValue
            ? _context.Targets.Skip((page.Value - 1) * count).Take(count)
            : _context.Targets;

        var targets = await targetQuery.AsNoTracking().ToListAsync();

        return new TargetPaginatedResponse()
        {
            Targets = targets.Select(target => target.ToTargetDto()),
            AvailableCount = availableCount
        };
    }

    public async Task<TargetDto> GetAsync(Guid id)
    {
        var targetTarget = await _context.Targets.FindAsync(id);
        if (targetTarget is null)
        {
            throw new TargetNotFoundException(id.ToString());
        }

        return targetTarget.ToTargetDto();
    }

    public async Task<TargetPaginatedResponse> GetByResourceIdAsync(Guid id, int? page, int count)
    {
        if (page.HasValue && page.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), $"Expected range: {nameof(page)} > 0.");
        }

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Expected range: {nameof(count)} > 0.");
        }

        var availableCount = await _context.Targets
            .Where(target => target.ResourceId == id)
            .CountAsync();

        // todo only fetch values if available count is more than 0
        var targetQuery = page.HasValue
            ? _context.Targets.Where(target => target.ResourceId == id)
                .Skip((page.Value - 1) * count)
                .Take(count)
            : _context.Targets.Where(target => target.ResourceId == id);

        var targets = await targetQuery.AsNoTracking().ToListAsync();

        return new TargetPaginatedResponse()
        {
            Targets = targets.Select(target => target.ToTargetDto()),
            AvailableCount = availableCount
        };
    }

    public async Task<TargetDto> CreateAsync(CreateTargetRequest request)
    {
        var targetResource = await _context.Resources.FindAsync(request.ResourceId);
        if (targetResource is null)
        {
            throw new ResourceNotFoundException(request.ResourceId.ToString());
        }

        var targetEntity = new TargetEntity()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            ResourceId = request.ResourceId,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Url = request.Url,
            CronSchedule = request.CronSchedule,
            ChangeType = request.ChangeType,
            HtmlTag = request.HtmlTag,
            SelectorType = request.SelectorType,
            SelectorValue = request.SelectorValue,
            ExpectedValue = request.ExpectedValue,
            CreatedAt = DateTime.UtcNow
        };

        await _jobService.ScheduleAsync(targetEntity.ToTarget());

        _context.Targets.Add(targetEntity);
        await _context.SaveChangesAsync();

        return targetEntity.ToTargetDto();
    }

    public async Task<TargetDto> UpdateAsync(Target updatedTarget)
    {
        var targetResource = await _context.Resources.FindAsync(updatedTarget.ResourceId);
        if (targetResource is null)
        {
            throw new ResourceNotFoundException(updatedTarget.ResourceId.ToString());
        }

        var targetTarget = await _context.Targets.FindAsync(updatedTarget.Id);
        if (targetTarget is null)
        {
            throw new TargetNotFoundException(updatedTarget.Id.ToString());
        }

        await _jobService.UnscheduleByTargetAsync(updatedTarget.Id, updatedTarget.ResourceId);
        await _jobService.ScheduleAsync(updatedTarget);

        _context.Entry(targetTarget).CurrentValues.SetValues(updatedTarget.ToTargetEntity());
        await _context.SaveChangesAsync();

        return targetTarget.ToTargetDto();
    }

    public async Task RemoveAsync(Guid id)
    {
        var targetTarget = await _context.Targets.FindAsync(id);
        if (targetTarget is null)
        {
            throw new TargetNotFoundException(targetTarget.Id.ToString());
        }

        await _jobService.UnscheduleByTargetAsync(targetTarget.Id, targetTarget.ResourceId);

        _context.Targets.Remove(targetTarget);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveByResourceIdAsync(Guid id)
    {
        var availableCount = await _context.Targets
            .Where(target => target.ResourceId == id)
            .CountAsync();

        if (availableCount == 0) 
        {
            throw new InvalidOperationException("No targets found for the given resource id: {id}.");
        }

        await _jobService.UnscheduleByResourceAsync(id);

        await _context.Targets.Where(target => target.ResourceId == id)
            .ExecuteDeleteAsync();
    }
}
