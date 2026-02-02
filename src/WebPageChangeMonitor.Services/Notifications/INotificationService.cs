using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Notifications;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Services.Notifications;

public interface INotificationService
{
    Task SendAsync(NotificationChannel channel, NotificationMessage message);
}
