using System.Text.Json.Serialization;

namespace WebPageChangeMonitor.Models.Notifications;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationBodyType
{
    PlainText,
    KeyValue
}
