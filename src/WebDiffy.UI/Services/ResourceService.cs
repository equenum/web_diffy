using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebDiffy.UI.Infrastructure.Options;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Responses;

namespace WebDiffy.UI.Services;

public interface IResourceService
{
    Task<ResourceDto> CreateAsync(ResourceDto resource);
    Task<ResourcePaginatedResponse> GetAsync(int? page = null, int? count = null);
}

public class ResourceService : BaseService, IResourceService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _resourcesBaseUrl;

    public ResourceService(IHttpClientFactory clientFactory, IOptions<ApiOptions> options) 
        : base(options)
    {
        _clientFactory = clientFactory;
        _resourcesBaseUrl = $"{BaseUrl}/api/public/resources";
    }

    public async Task<ResourceDto> CreateAsync(ResourceDto resource)
    {
        var message = BuildPostRequestMessage(_resourcesBaseUrl, resource);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ResourceDto>();
    }

    public async Task<ResourcePaginatedResponse> GetAsync(int? page = null, int? count = null)
    {
        var queryParams = new Dictionary<string, string>();

        if (page.HasValue)
        {
            queryParams.Add("page", page.ToString());
        }

        if (count.HasValue)
        {
            queryParams.Add("count", count.ToString());
        }

        var message = BuildGetRequestMessage(_resourcesBaseUrl, queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ResourcePaginatedResponse>();
    }
}
