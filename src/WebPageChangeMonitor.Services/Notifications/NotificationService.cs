using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebPageChangeMonitor.Models.Notifications;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHttpClientFactory clientFactory,
        ILogger<NotificationService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task SendAsync(string tenantId, NotificationChannel channel, NotificationMessage message)
    {
        // skip auth, if tenant id, token or auth header are undefined.
        // log if auth skipped.
        throw new NotImplementedException();
    }
}
