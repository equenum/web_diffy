namespace WebPageChangeMonitor.Models.Options;

public class NotificationChannel
{
    public string Name { get; init; }
    public bool IsEnabled { get; init; }
    public string Url { get; init; }
    public string TenantId { get; init; }
    public string TenantToken { get; init; }
    public string TenantIdHeader { get; init; }
    public string TenantTokenHeader { get; init; }
}
