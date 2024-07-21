using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebPageChangeMonitor.Api.Controller;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Infrastructure.Mappers;
using WebPageChangeMonitor.Api.Models.Responses;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Dtos;

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

    public async Task<TargetSnapshotPaginatedResponse> GetByTargetIdAsync(Guid id, int? page, int count)
    {
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

        // todo only fetch values if available count is more than 0
        var snapshotQuery = page.HasValue
            ? _context.TargetSnapshots.Where(snapshot => snapshot.TargetId == id)
                .Skip((page.Value - 1) * count)
                .Take(count)
            : _context.TargetSnapshots.Where(snapshot => snapshot.TargetId == id);

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
