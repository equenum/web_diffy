using System.Text.Json.Serialization;

namespace WebPageChangeMonitor.Models.Consts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChangeType
{
    Value,
    Snapshot
}
