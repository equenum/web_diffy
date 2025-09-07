using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Notifications;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Services.Notifications;

public interface INotificationService
{
    Task SendAsync(string tenantId, NotificationChannel channel, NotificationMessage message);
}
