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

    public async Task<TargetPaginatedResponse> GetAsync(SortDirection? sortDirection, string sortBy,
        int? page, int count)
    {
        if (sortDirection.HasValue)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                throw new ArgumentException("Value cannot be null", nameof(sortBy));
            }

            if (typeof(TargetEntity).GetProperty(sortBy) is null)
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

        var availableCount = await _context.Targets.CountAsync();
        if (availableCount == 0)
        {
            return new TargetPaginatedResponse()
            {
                Targets = [],
                AvailableCount = availableCount
            };
        }

        IQueryable<TargetEntity> targetQuery = _context.Targets;

        if (sortDirection.HasValue)
        {
            var propertyLambda = ExpressionHelper.GetPropertyLambda<TargetEntity>(sortBy);

            targetQuery = sortDirection switch
            {
                SortDirection.Asc => targetQuery.OrderBy(propertyLambda),
                SortDirection.Desc => targetQuery.OrderByDescending(propertyLambda),
                _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), "Unexpected sort direction value."),
            };
        }

        targetQuery = page.HasValue
            ? targetQuery.Skip((page.Value - 1) * count).Take(count)
            : targetQuery.Take(count);

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

    public async Task<TargetPaginatedResponse> GetByResourceIdAsync(Guid id, SortDirection? sortDirection,
        string sortBy, int? page, int count)
    {
        if (sortDirection.HasValue)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                throw new ArgumentException("Value cannot be null", nameof(sortBy));
            }

            if (typeof(TargetEntity).GetProperty(sortBy) is null)
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

        var availableCount = await _context.Targets
            .Where(target => target.ResourceId == id)
            .CountAsync();

        if (availableCount == 0)
        {
            return new TargetPaginatedResponse()
            {
                Targets = [],
                AvailableCount = availableCount
            };
        }

        IQueryable<TargetEntity> targetQuery = _context.Targets.Where(target => target.ResourceId == id);

        if (sortDirection.HasValue)
        {
            var propertyLambda = ExpressionHelper.GetPropertyLambda<TargetEntity>(sortBy);

            targetQuery = sortDirection switch
            {
                SortDirection.Asc => targetQuery.OrderBy(propertyLambda),
                SortDirection.Desc => targetQuery.OrderByDescending(propertyLambda),
                _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), "Unexpected sort direction value."),
            };
        }

        targetQuery = page.HasValue
            ? targetQuery.Skip((page.Value - 1) * count).Take(count)
            : targetQuery.Take(count);

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

        _context.Targets.Add(targetEntity);
        await _context.SaveChangesAsync();

        await _jobService.ScheduleAsync(targetEntity.ToTarget());

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

        _context.Entry(targetTarget).CurrentValues.SetValues(updatedTarget.ToTargetEntity());
        await _context.SaveChangesAsync();

        await _jobService.UnscheduleByTargetAsync(updatedTarget.Id, updatedTarget.ResourceId);
        await _jobService.ScheduleAsync(updatedTarget);

        return targetTarget.ToTargetDto();
    }

    public async Task RemoveAsync(Guid id)
    {
        var targetTarget = await _context.Targets.FindAsync(id);
        if (targetTarget is null)
        {
            throw new TargetNotFoundException(targetTarget.Id.ToString());
        }

        _context.Targets.Remove(targetTarget);
        await _context.SaveChangesAsync();
        
        await _jobService.UnscheduleByTargetAsync(targetTarget.Id, targetTarget.ResourceId);
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

        await _context.Targets.Where(target => target.ResourceId == id).ExecuteDeleteAsync();
        await _jobService.UnscheduleByResourceAsync(id);
    }
}
