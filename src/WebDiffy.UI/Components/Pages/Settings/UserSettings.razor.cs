using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure.Helpers;
using WebDiffy.UI.Infrastructure.UserSettings;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Settings;

public partial class UserSettings
{
    private const string ArrayDelimeter = ", ";

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IUserSettingsService UserSettingsService { get; set; }

    private UserSettingsDto CopiedUserSettings;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        CopiedUserSettings = ObjectCopyHelper.DeepCopy(UserAppSettings.ToData());
    }

    private static string ConvertArrayBasedValue<T>(T[] items) where T : struct
    {
        return string.Join(ArrayDelimeter, items.Select(item => item.ToString()));
    }

    private void UpdateGridPageSizeOptions(string value)
    {
        CopiedUserSettings.GridPageSizeOptions = [.. value.Split(ArrayDelimeter).Select(int.Parse)];
    }

    private void UpdateLargeGridPageSizeOptions(string value)
    {
        CopiedUserSettings.LargeGridPageSizeOptions = [.. value.Split(ArrayDelimeter).Select(int.Parse)];
    }

    private void Reset()
    {
        UserAppSettings.Reset();
        CopiedUserSettings = ObjectCopyHelper.DeepCopy(UserAppSettings.ToData());
    }

    private async void Save()
    {
        try
        {
            CopiedUserSettings = await UserSettingsService.UpdateAsync(CopiedUserSettings);

            UserAppSettings.Populate(CopiedUserSettings);
            Snackbar.Add("Settings saved", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Save Settings' failed", Severity.Error);
        }
    }
}
