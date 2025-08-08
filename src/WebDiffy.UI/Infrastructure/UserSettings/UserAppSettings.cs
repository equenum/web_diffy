namespace WebDiffy.UI.Infrastructure.UserSettings;

public static class UserAppSettings
{
    public const string Key = "userSettings";

    private static bool _wasModified = false;
    public static bool WasModified { get => _wasModified; }

    private static bool _areDeletePopUpsEnabled = true;
    public static bool AreDeletePopUpsEnabled
    {
        get => _areDeletePopUpsEnabled;
        set
        {
            _areDeletePopUpsEnabled = value;
            _wasModified = true;
        }
    }

    private static bool _areItemIdColumnsEnabled = true;
    public static bool AreItemIdColumnsEnabled
    {
        get => _areItemIdColumnsEnabled;
        set
        {
            _areItemIdColumnsEnabled = value;
            _wasModified = true;
        }
    }

    private static bool _areItemIdsShortened = true;
    public static bool AreItemIdsShortened
    {
        get => _areItemIdsShortened;
        set
        {
            _areItemIdsShortened = value;
            _wasModified = true;
        }
    }

    private static bool _areCollapsePanelsExpanded = true;
    public static bool AreCollapsePanelsExpanded
    {
        get => _areCollapsePanelsExpanded;
        set
        {
            _areCollapsePanelsExpanded = value;
            _wasModified = true;
        }
    }

    private static int _itemIdSubSectionLength = 12;
    public static int ItemIdSubSectionLength
    {
        get => _itemIdSubSectionLength;
        set
        {
            _itemIdSubSectionLength = value;
            _wasModified = true;
        }
    }

    private static int[] _gridPageSizeOptions = [10, 25, 50, 100];
    public static int[] GridPageSizeOptions
    {
        get => _gridPageSizeOptions;
        set
        {
            _gridPageSizeOptions = value;
            _wasModified = true;
        }
    }

    private static int[] _largeGridPageSizeOptions = [50, 100, 200];
    public static int[] LargeGridPageSizeOptions
    {
        get => _largeGridPageSizeOptions;
        set
        {
            _largeGridPageSizeOptions = value;
            _wasModified = true;
        }
    }

    public static void Reset()
    {
        AreDeletePopUpsEnabled = true;
        AreItemIdColumnsEnabled = true;
        AreItemIdsShortened = true;
        AreCollapsePanelsExpanded = true;
        ItemIdSubSectionLength = 12;
        GridPageSizeOptions = [10, 25, 50, 100];
        LargeGridPageSizeOptions = [50, 100, 200];
    }

    public static void ResetModifiedStatus()
    {
        _wasModified = false;
    }

    public static void Populate(UserAppSettingsData data)
    {
        _areDeletePopUpsEnabled = data.AreDeletePopUpsEnabled;
        _areItemIdColumnsEnabled = data.AreItemIdColumnsEnabled;
        _areItemIdsShortened = data.AreItemIdsShortened;
        _areCollapsePanelsExpanded = data.AreCollapsePanelsExpanded;
        _itemIdSubSectionLength = data.ItemIdSubSectionLength;
        _gridPageSizeOptions = data.GridPageSizeOptions;
        _largeGridPageSizeOptions = data.LargeGridPageSizeOptions;
    }

    public static UserAppSettingsData ToData()
    {
        return new UserAppSettingsData()
        {
            AreDeletePopUpsEnabled = AreDeletePopUpsEnabled,
            AreItemIdColumnsEnabled = AreItemIdColumnsEnabled,
            AreItemIdsShortened = AreItemIdsShortened,
            AreCollapsePanelsExpanded = AreCollapsePanelsExpanded,
            ItemIdSubSectionLength = ItemIdSubSectionLength,
            GridPageSizeOptions = GridPageSizeOptions,
            LargeGridPageSizeOptions = LargeGridPageSizeOptions
        };
    }
}
