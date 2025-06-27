using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface ITargetSnapshotService
{
    Task<TargetSnapshotDto> GetAsync(Guid id);
    Task<TargetSnapshotPaginatedResponse> GetByTargetIdAsync(Guid id, int? page, int count);
    Task RemoveAsync(Guid id);
    Task RemoveByTargetIdAsync(Guid id);
}
