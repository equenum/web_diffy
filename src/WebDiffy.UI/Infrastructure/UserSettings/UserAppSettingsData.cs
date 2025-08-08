namespace WebDiffy.UI.Infrastructure.UserSettings;

public class UserAppSettingsData
{
    public bool AreDeletePopUpsEnabled { get; set; }
    public bool AreItemIdColumnsEnabled { get; set; }
    public bool AreItemIdsShortened { get; set; }
    public bool AreCollapsePanelsExpanded { get; set; }
    public int ItemIdSubSectionLength { get; set; }
    public int[] GridPageSizeOptions { get; set; }
    public int[] LargeGridPageSizeOptions { get; set; }
}
