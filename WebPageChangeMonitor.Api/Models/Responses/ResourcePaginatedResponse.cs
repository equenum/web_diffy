using System.Collections.Generic;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Responses;

public class ResourcePaginatedResponse
{
    public IEnumerable<ResourceDto> Resources { get; set; }
    public int AvailableCount { get; set; }
}
