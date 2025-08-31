using System.Collections.Generic;

namespace WebPageChangeMonitor.Models.Options;

public class NotificationOptions
{
    public string TenantId { get; init; }
    public List<NotificationChannel> Channels { get; init; }
}
