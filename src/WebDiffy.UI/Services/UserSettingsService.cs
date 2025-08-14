using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebDiffy.UI.Infrastructure.Options;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Services;

public interface IUserSettingsService
{
    Task<UserSettingsDto> GetAsync();
    Task<UserSettingsDto> CreateAsync(UserSettingsDto settings);
    Task<UserSettingsDto> UpdateAsync(UserSettingsDto updatedSettings);
}

public class UserSettingsService : BaseService, IUserSettingsService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _userSettingsBaseUrl;

    public UserSettingsService(IHttpClientFactory clientFactory, IOptions<ApiOptions> options) 
        : base(options)
    {
        _clientFactory = clientFactory;
        _userSettingsBaseUrl = $"{BaseUrl}/api/public/user-settings";
    }

    public async Task<UserSettingsDto> GetAsync()
    {
        var message = BuildGetRequestMessage(_userSettingsBaseUrl);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserSettingsDto>();
    }

    public async Task<UserSettingsDto> CreateAsync(UserSettingsDto settings)
    {
        var message = BuildPostRequestMessage(_userSettingsBaseUrl, settings);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserSettingsDto>();
    }

    public async Task<UserSettingsDto> UpdateAsync(UserSettingsDto updatedSettings)
    {
        var message = BuildPutRequestMessage(_userSettingsBaseUrl, updatedSettings);
        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserSettingsDto>();
    }
}
