using System;
using System.Linq;
using WebDiffy.UI.Infrastructure.UserSettings;

namespace WebDiffy.UI.Infrastructure.Helpers;

public static class GuidHelper
{
    public static string ConvertToString(Guid guid)
    {
        var stringGuid = guid.ToString();

        if (UserAppSettings.AreItemIdsShortened)
        {
            var subString = stringGuid.ToCharArray().TakeLast(UserAppSettings.ItemIdSubSectionLength);
            return string.Join(string.Empty, subString);
        }

        return stringGuid;
    }
}
