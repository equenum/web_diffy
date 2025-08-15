namespace WebPageChangeMonitor.Models.Dtos;

public class UserSettingsDto
{
    public bool AreDeletePopUpsEnabled { get; set; }
    public bool AreItemIdColumnsEnabled { get; set; }
    public bool AreItemIdsShortened { get; set; }
    public bool AreTargetsCreatedAsActive { get; set; }
    public int ItemIdSubSectionLength { get; set; }
    public int[] GridPageSizeOptions { get; set; }
    public int[] LargeGridPageSizeOptions { get; set; }
}
