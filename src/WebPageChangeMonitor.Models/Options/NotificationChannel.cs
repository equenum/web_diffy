namespace WebPageChangeMonitor.Models.Options;

public class NotificationChannel
{
    public string Name { get; init; }
    public bool IsEnabled { get; init; }
    public string Url { get; init; }
    public string Token { get; init; }
}
