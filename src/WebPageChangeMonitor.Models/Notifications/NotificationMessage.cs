using System;

namespace WebPageChangeMonitor.Models.Notifications;

public class NotificationMessage
{
    public string Title { get; set; }
    public string Body { get; set; }
    public NotificationBodyType BodyType { get; set; }
    public string Origin { get; set; }
    public DateTime Timestamp { get; set; }
}
