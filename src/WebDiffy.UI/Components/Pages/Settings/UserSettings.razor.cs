using System.Linq;
using WebDiffy.UI.Infrastructure;

namespace WebDiffy.UI.Components.Pages.Settings;

public partial class UserSettings
{
    private const string ArrayDelimeter = ", ";

    private static string ConvertArrayBasedValue<T>(T[] items) where T : struct
    {
        return string.Join(ArrayDelimeter, items.Select(item => item.ToString()));
    }

    private static void UpdateGridPageSizeOptions(string value)
    {
        UserAppSettings.GridPageSizeOptions = [.. value.Split(ArrayDelimeter).Select(int.Parse)];
    }

    private static void Reset()
    {
        UserAppSettings.Reset();
    }
}
