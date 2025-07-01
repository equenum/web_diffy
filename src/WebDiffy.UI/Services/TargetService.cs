using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebDiffy.UI.Infrastructure.Options;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Responses;

namespace WebDiffy.UI.Services;

public interface ITargetService
{
    Task<TargetDto> CreateAsync(TargetDto target);
    Task<TargetPaginatedResponse> GetAsync(int? page = null, int? count = null);
    Task<TargetDto> GetAsync(Guid id);
    Task<TargetPaginatedResponse> GetByResourceAsync(Guid resourceId, int? page = null, int? count = null);
    Task<TargetDto> UpdateAsync(TargetDto target);
    Task RemoveAsync(Guid id);
    Task RemoveByResourceAsync(Guid resourceId);
}

public class TargetService : BaseService, ITargetService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _targetsBaseUrl;

    public TargetService(IHttpClientFactory clientFactory, IOptions<ApiOptions> options) 
        : base(options)
    {
        _clientFactory = clientFactory;
        _targetsBaseUrl = $"{BaseUrl}/api/public/targets";
    }

    public async Task<TargetDto> CreateAsync(TargetDto target)
    {
        var message = BuildPostRequestMessage(_targetsBaseUrl, target);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetDto>();
    }

    public async Task<TargetPaginatedResponse> GetAsync(int? page = null, int? count = null)
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

        var message = BuildGetRequestMessage(_targetsBaseUrl, queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetPaginatedResponse>();
    }

    public async Task<TargetDto> GetAsync(Guid id)
    {
        var queryParams = new Dictionary<string, string>() {{ "id", id.ToString() }};

        var message = BuildGetRequestMessage(_targetsBaseUrl, queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetDto>();
    }

    public async Task<TargetPaginatedResponse> GetByResourceAsync(Guid resourceId, int? page = null, int? count = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "id", resourceId.ToString() }
        };

        if (page.HasValue)
        {
            queryParams.Add("page", page.ToString());
        }

        if (count.HasValue)
        {
            queryParams.Add("count", count.ToString());
        }

        var message = BuildGetRequestMessage($"{_targetsBaseUrl}/resource", queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetPaginatedResponse>();
    }

    public async Task RemoveAsync(Guid id)
    {
        var queryParams = new Dictionary<string, string>() {{ "id", id.ToString() }};

        var message = BuildDeleteRequestMessage(_targetsBaseUrl, queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveByResourceAsync(Guid resourceId)
    {
        var queryParams = new Dictionary<string, string>() {{ "id", resourceId.ToString() }};

        var message = BuildDeleteRequestMessage($"{_targetsBaseUrl}/resource", queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TargetDto> UpdateAsync(TargetDto target)
    {
        var message = BuildPutRequestMessage(_targetsBaseUrl, target);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetDto>();
    }
}
