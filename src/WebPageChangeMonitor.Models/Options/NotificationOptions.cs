using System.Collections.Generic;

namespace WebPageChangeMonitor.Models.Options;

public class NotificationOptions
{
    public string OriginName { get; init; }
    public bool AreEnabled { get; init; }
    public List<NotificationChannel> Channels { get; init; }
}
