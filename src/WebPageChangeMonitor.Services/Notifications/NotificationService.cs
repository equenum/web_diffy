using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

    public async Task SendAsync(NotificationChannel channel, NotificationMessage message)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, channel.Url);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8);
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var wasAuthApplied = false;

        if (IsAuthEnabled(channel) && IsHeaderConfigValid(channel))
        {
            requestMessage.Content.Headers.Add(channel.TenantIdHeader, channel.TenantId);
            requestMessage.Content.Headers.Add(channel.TenantTokenHeader, channel.TenantToken);

            wasAuthApplied = true;
        }

        if (!wasAuthApplied)
        {
            _logger.LogInformation("Tenant authentication skipped for channel '{ChannelName}'",
                channel.Name);
        }

        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();
    }

    private static bool IsAuthEnabled(NotificationChannel channel) =>
        !string.IsNullOrWhiteSpace(channel.TenantId)
        && !string.IsNullOrWhiteSpace(channel.TenantToken);

    private static bool IsHeaderConfigValid(NotificationChannel channel) => 
        !string.IsNullOrWhiteSpace(channel.TenantIdHeader)
        && !string.IsNullOrWhiteSpace(channel.TenantTokenHeader);
}
