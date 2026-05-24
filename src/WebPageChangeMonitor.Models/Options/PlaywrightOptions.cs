namespace WebPageChangeMonitor.Models.Options;

public class PlaywrightOptions
{
    public string UserAgent { get; set; }
    public string ChromiumVersion { get; set; }
    public string GoogleChromeVersion { get; set; }
    public string GreaseVersion { get; set; }
    public string Platform { get; set; }
    public bool IsMobile { get; set; }
}
