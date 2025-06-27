using System.Collections.Generic;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Models.Responses;

public class TargetSnapshotPaginatedResponse
{
    public IEnumerable<TargetSnapshotDto> Snapshots { get; set; }
    public int AvailableCount { get; set; }
}
