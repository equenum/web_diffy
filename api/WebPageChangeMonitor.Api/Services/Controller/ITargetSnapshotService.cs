using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Api.Models.Responses;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface ITargetSnapshotService
{
    Task<TargetSnapshotDto> GetAsync(Guid id);
    Task<TargetSnapshotPaginatedResponse> GetByTargetIdAsync(Guid id, int? page, int count);
    Task RemoveAsync(Guid id);
    Task RemoveByTargetIdAsync(Guid id);
}
