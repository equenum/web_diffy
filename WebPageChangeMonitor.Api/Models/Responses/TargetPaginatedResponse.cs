using System.Collections.Generic;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Models.Responses;

public class TargetPaginatedResponse
{
    public IEnumerable<TargetDto> Targets { get; set; }
    public int AvailableCount { get; set; }
}
