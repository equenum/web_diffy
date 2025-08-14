using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Dtos;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface IUserSettingsService
{
    Task<UserSettingsDto> GetAsync();
    Task<UserSettingsDto> CreateAsync(UserSettingsDto settings);
    Task<UserSettingsDto> UpdateAsync(UserSettingsDto updatedSettings);
}
