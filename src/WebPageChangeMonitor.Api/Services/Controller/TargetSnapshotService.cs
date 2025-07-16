using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Common.Helpers;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Services.Controller;

public class TargetSnapshotService : ITargetSnapshotService
{
    private readonly MonitorDbContext _context;

    public TargetSnapshotService(MonitorDbContext context)
    {
        _context = context;
    }

    public async Task<TargetSnapshotDto> GetAsync(Guid id)
    {
        var targetSnapshot = await _context.TargetSnapshots.FindAsync(id);
        if (targetSnapshot is null)
        {
            throw new TargetSnapshotNotFoundException(id.ToString());
        }

        return targetSnapshot.ToTargetSnapshotDto();
    }

    public async Task<TargetSnapshotPaginatedResponse> GetByTargetIdAsync(Guid id, SortDirection? sortDirection,
        string sortBy, int? page, int count)
    {
        if (sortDirection.HasValue)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                throw new ArgumentException("Value cannot be null", nameof(sortBy));
            }

            if (typeof(TargetSnapshotEntity).GetProperty(sortBy) is null)
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

        var availableCount = await _context.TargetSnapshots
            .Where(snapshot => snapshot.TargetId == id)
            .CountAsync();

        if (availableCount == 0)
        {
            return new TargetSnapshotPaginatedResponse()
            {
                Snapshots = [],
                AvailableCount = availableCount
            };
        }

        IQueryable<TargetSnapshotEntity> snapshotQuery = _context.TargetSnapshots.Where(snapshot => snapshot.TargetId == id);

        if (sortDirection.HasValue)
        {
            var propertyLambda = ExpressionHelper.GetPropertyLambda<TargetSnapshotEntity>(sortBy);

            snapshotQuery = sortDirection switch
            {
                SortDirection.Asc => snapshotQuery.OrderBy(propertyLambda),
                SortDirection.Desc => snapshotQuery.OrderByDescending(propertyLambda),
                _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), "Unexpected sort direction value."),
            };
        }

        snapshotQuery = page.HasValue
            ? snapshotQuery.Skip((page.Value - 1) * count).Take(count)
            : snapshotQuery.Take(count);

        var snapshots = await snapshotQuery.AsNoTracking().ToListAsync();

        return new TargetSnapshotPaginatedResponse()
        {
            Snapshots = snapshots.Select(snapshot => snapshot.ToTargetSnapshotDto()),
            AvailableCount = availableCount
        };
    }

    public async Task RemoveAsync(Guid id)
    {
        var targetSnapshot = await _context.TargetSnapshots.FindAsync(id);
        if (targetSnapshot is null)
        {
            throw new TargetSnapshotNotFoundException(targetSnapshot.Id.ToString());
        }

        _context.TargetSnapshots.Remove(targetSnapshot);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveByTargetIdAsync(Guid id)
    {
        var availableCount = await _context.TargetSnapshots
            .Where(snapshot => snapshot.TargetId == id)
            .CountAsync();

        if (availableCount == 0) 
        {
            throw new InvalidOperationException("No target snapshots found for the given target id: {id}.");
        }

        await _context.TargetSnapshots.Where(snapshot => snapshot.TargetId == id)
            .ExecuteDeleteAsync();
    }
}
