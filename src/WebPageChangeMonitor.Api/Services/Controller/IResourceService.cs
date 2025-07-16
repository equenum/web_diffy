using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Requests;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface IResourceService
{
    Task<ResourcePaginatedResponse> GetAsync(SortDirection? sortDirection, string sortBy, int? page, int count);
    Task<ResourceDto> GetAsync(Guid id);
    Task<ResourceDto> CreateAsync(CreateResourceRequest request);
    Task<ResourceDto> UpdateAsync(Resource updatedResource);
    Task RemoveAsync(Guid id);
}
