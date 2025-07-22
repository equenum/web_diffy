namespace WebDiffy.UI.Infrastructure;

public static class UserAppSettings
{
    public static bool AreDeletePopUpsEnabled { get; set; } = true;
    public static bool AreItemIdColumnsEnabled { get; set; } = true;
    public static bool AreItemIdsShortened { get; set; } = true;
    public static bool AreCollapsePanelsExpanded { get; set; } = false;
    public static int ItemIdSubSectionLength { get; set; } = 12;
    public static int[] GridPageSizeOptions { get; set; } = [10, 25, 50, 100];

    public static void Reset()
    {
        AreDeletePopUpsEnabled = true;
        AreItemIdColumnsEnabled = true;
        AreItemIdsShortened = true;
        AreCollapsePanelsExpanded = false;
        ItemIdSubSectionLength = 12;
        GridPageSizeOptions = [10, 25, 50, 100];
    }
}
