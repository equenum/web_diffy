using MudBlazor;

namespace WebDiffy.UI;

public static class ThemeConfigs
{
    public const bool IsDarkMode = true;

    public static MudTheme Theme
    {
        get
        {
            var theme = new MudTheme();
            string[] customfontFamily = ["Ubuntu", "Helvetica", "Arial", "sans-serif"];

            theme.Typography.H1.FontFamily = customfontFamily;
            theme.Typography.H2.FontFamily = customfontFamily;
            theme.Typography.H3.FontFamily = customfontFamily;
            theme.Typography.H4.FontFamily = customfontFamily;
            theme.Typography.H5.FontFamily = customfontFamily;
            theme.Typography.H6.FontFamily = customfontFamily;

            theme.Typography.Subtitle1.FontFamily = customfontFamily;
            theme.Typography.Subtitle2.FontFamily = customfontFamily;

            theme.Typography.Body1.FontFamily = customfontFamily;
            theme.Typography.Body2.FontFamily = customfontFamily;

            theme.Typography.Button.FontFamily = customfontFamily;
            theme.Typography.Caption.FontFamily = customfontFamily;
            theme.Typography.Overline.FontFamily = customfontFamily;
            theme.Typography.Default.FontFamily = customfontFamily;

            return theme;
        }
    }
}