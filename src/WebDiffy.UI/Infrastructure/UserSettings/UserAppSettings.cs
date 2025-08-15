using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Infrastructure.UserSettings;

public static class UserAppSettings
{
    public static bool AreDeletePopUpsEnabled { get; set; } = true;
    public static bool AreItemIdColumnsEnabled { get; set; } = true;
    public static bool AreItemIdsShortened { get; set; } = true;
    public static bool AreTargetsCreatedAsActive { get; set; } = false;
    public static int ItemIdSubSectionLength { get; set; } = 12;
    public static int[] GridPageSizeOptions { get; set; } = [10, 25, 50, 100];
    public static int[] LargeGridPageSizeOptions { get; set; } = [50, 100, 200];

    public static void Reset()
    {
        AreDeletePopUpsEnabled = true;
        AreItemIdColumnsEnabled = true;
        AreItemIdsShortened = true;
        AreTargetsCreatedAsActive = false;
        ItemIdSubSectionLength = 12;
        GridPageSizeOptions = [10, 25, 50, 100];
        LargeGridPageSizeOptions = [50, 100, 200];
    }

    public static void Populate(UserSettingsDto dto)
    {
        AreDeletePopUpsEnabled = dto.AreDeletePopUpsEnabled;
        AreItemIdColumnsEnabled = dto.AreItemIdColumnsEnabled;
        AreItemIdsShortened = dto.AreItemIdsShortened;
        AreTargetsCreatedAsActive = dto.AreTargetsCreatedAsActive;
        ItemIdSubSectionLength = dto.ItemIdSubSectionLength;
        GridPageSizeOptions = dto.GridPageSizeOptions;
        LargeGridPageSizeOptions = dto.LargeGridPageSizeOptions;
    }

    public static UserSettingsDto ToData()
    {
        return new UserSettingsDto()
        {
            AreDeletePopUpsEnabled = AreDeletePopUpsEnabled,
            AreItemIdColumnsEnabled = AreItemIdColumnsEnabled,
            AreItemIdsShortened = AreItemIdsShortened,
            AreTargetsCreatedAsActive = AreTargetsCreatedAsActive,
            ItemIdSubSectionLength = ItemIdSubSectionLength,
            GridPageSizeOptions = GridPageSizeOptions,
            LargeGridPageSizeOptions = LargeGridPageSizeOptions
        };
    }
}
