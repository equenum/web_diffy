using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Api.Models.Requests;
using WebPageChangeMonitor.Api.Models.Responses;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface ITargetService
{
    Task<TargetPaginatedResponse> GetAsync(int? page, int count);
    Task<TargetDto> GetAsync(Guid id);
    Task<TargetPaginatedResponse> GetByResourceIdAsync(Guid id, int? page, int count);
    Task<TargetDto> CreateAsync(CreateTargetRequest request);
    Task<TargetDto> UpdateAsync(Target updatedTarget);
    Task RemoveAsync(Guid id);
    Task RemoveByResourceIdAsync(Guid id);
}
