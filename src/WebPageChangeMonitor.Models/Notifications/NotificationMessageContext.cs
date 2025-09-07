using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Models.Notifications;

public class NotificationMessageContext
{
    public string Message { get; set; }
    public bool IsChangeDetected { get; set; }
    public TargetSnapshotEntity Snapshot { get; set; }
}
