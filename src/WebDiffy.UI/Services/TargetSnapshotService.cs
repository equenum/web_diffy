using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebDiffy.UI.Infrastructure.Options;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Responses;

namespace WebDiffy.UI.Services;

public interface ITargetSnapshotService
{
    Task<TargetSnapshotDto> GetAsync(Guid id);
    Task<TargetSnapshotPaginatedResponse> GetByTargetAsync(
        Guid targetId, int? page = null, int? count = null, SortDirection? sortDirection = null, string sortBy = null);
    Task RemoveAsync(Guid id);
    Task RemoveByTargetAsync(Guid targetId);
}

public class TargetSnapshotService : BaseService, ITargetSnapshotService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _targetSnapshotsBaseUrl;

    public TargetSnapshotService(IHttpClientFactory clientFactory, IOptions<ApiOptions> options) 
        : base(options)
    {
        _clientFactory = clientFactory;
        _targetSnapshotsBaseUrl = $"{BaseUrl}/api/public/snapshots";
    }

    public async Task<TargetSnapshotDto> GetAsync(Guid id)
    {
        var message = BuildGetRequestMessage($"{_targetSnapshotsBaseUrl}/{id}");
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetSnapshotDto>();
    }

    public async Task<TargetSnapshotPaginatedResponse> GetByTargetAsync(Guid targetId, int? page = null,
        int? count = null, SortDirection? sortDirection = null, string sortBy = null)
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

        if (sortDirection.HasValue && !string.IsNullOrWhiteSpace(sortBy))
        {
            queryParams.Add("sortDirection", sortDirection.ToString());
            queryParams.Add("sortBy", sortBy);
        }

        var message = BuildGetRequestMessage($"{_targetSnapshotsBaseUrl}/target/{targetId}", queryParams);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TargetSnapshotPaginatedResponse>();
    }

    public async Task RemoveAsync(Guid id)
    {
        var message = BuildDeleteRequestMessage($"{_targetSnapshotsBaseUrl}/{id}");
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveByTargetAsync(Guid targetId)
    {
        var message = BuildDeleteRequestMessage($"{_targetSnapshotsBaseUrl}/target/{targetId}");
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }
}
