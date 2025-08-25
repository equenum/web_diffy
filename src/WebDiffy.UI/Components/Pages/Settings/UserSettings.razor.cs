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
    private const int MaxGridPageSize = 5;
    private const int MaxGridPageSizeValue = 100;
    private const int MaxLargeGridPageSizeValue = 1000;

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IUserSettingsService UserSettingsService { get; set; }

    private string GridPageSizeOptionsText = string.Empty;
    private string LargeGridPageSizeOptionsText = string.Empty;

    private UserSettingsDto CopiedUserSettings;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        CopiedUserSettings = ObjectCopyHelper.DeepCopy(UserAppSettings.ToData());

        GridPageSizeOptionsText = ConvertArrayBasedValue(CopiedUserSettings.GridPageSizeOptions);
        LargeGridPageSizeOptionsText = ConvertArrayBasedValue(CopiedUserSettings.LargeGridPageSizeOptions);
    }

    private static string ConvertArrayBasedValue<T>(T[] items) where T : struct
    {
        return string.Join(ArrayDelimeter, items.Select(item => item.ToString()));
    }

    private void UpdateGridPageSizeOptions(string value)
    {
        CopiedUserSettings.GridPageSizeOptions = [.. value.Trim().Split(ArrayDelimeter).Select(int.Parse)];
    }

    private void UpdateLargeGridPageSizeOptions(string value)
    {
        CopiedUserSettings.LargeGridPageSizeOptions = [.. value.Trim().Split(ArrayDelimeter).Select(int.Parse)];
    }

    private void Reset()
    {
        UserAppSettings.Reset();
        CopiedUserSettings = ObjectCopyHelper.DeepCopy(UserAppSettings.ToData());

        GridPageSizeOptionsText = ConvertArrayBasedValue(CopiedUserSettings.GridPageSizeOptions);
        LargeGridPageSizeOptionsText = ConvertArrayBasedValue(CopiedUserSettings.LargeGridPageSizeOptions);

        Form.ResetValidation();
    }

    private async void Save()
    {
        if (!Form.IsValid)
        {
            return;
        }

        try
        {
            UpdateGridPageSizeOptions(GridPageSizeOptionsText);
            UpdateLargeGridPageSizeOptions(LargeGridPageSizeOptionsText);

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
