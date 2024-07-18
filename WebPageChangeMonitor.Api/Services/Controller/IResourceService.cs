using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Api.Requests;
using WebPageChangeMonitor.Api.Responses;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface IResourceService
{
    Task<ResourcePaginatedResponse> GetAsync(int? page, int count);
    Task<ResourceDto> GetAsync(Guid id);
    Task<ResourceDto> CreateAsync(CreateRecourseRequest request);
    Task<ResourceDto> UpdateAsync(Resource updatedResource);
    Task RemoveAsync(Guid id);
}
