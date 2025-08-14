using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UUIDNext;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Data;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Api.Services.Controller;

public class UserSettingsService : IUserSettingsService
{
    private readonly MonitorDbContext _context;

    public UserSettingsService(MonitorDbContext context)
    {
        _context = context;
    }

    public async Task<UserSettingsDto> GetAsync()
    {
        var userSettings = await _context.UserSettings.FirstOrDefaultAsync();
        if (userSettings is null)
        {
            throw new UserSettingsNotFoundException();
        }

        return JsonSerializer.Deserialize<UserSettingsDto>(userSettings.Value);  
    }

    public async Task<UserSettingsDto> CreateAsync(UserSettingsDto settings)
    {
        var settingsEntity = new UserSettingsEntity()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Value = JsonSerializer.Serialize(settings),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSettings.Add(settingsEntity);
        await _context.SaveChangesAsync();

        return JsonSerializer.Deserialize<UserSettingsDto>(settingsEntity.Value);  
    }

    public async Task<UserSettingsDto> UpdateAsync(UserSettingsDto updatedSettings)
    {
        var userSettings = await _context.UserSettings.FirstOrDefaultAsync();
        if (userSettings is null)
        {
            throw new UserSettingsNotFoundException();
        }

        userSettings.Value = JsonSerializer.Serialize(updatedSettings);
        await _context.SaveChangesAsync();

        return JsonSerializer.Deserialize<UserSettingsDto>(userSettings.Value);  
    }
}
