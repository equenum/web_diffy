namespace WebDiffy.UI.Infrastructure;

public static class UserAppSettings
{
    public static bool AreDeletePopUpsEnabled { get; set; } = true;
    public static int[] GridPageSizeOptions { get; set; } = [10, 25, 50, 100];

    public static void Reset()
    {
        AreDeletePopUpsEnabled = true;
        GridPageSizeOptions = [10, 25, 50, 100];
    }
}
