using System.Text.Json;

namespace WebDiffy.UI.Infrastructure.Helpers;

public static class ObjectCopyHelper
{
    public static T DeepCopy<T>(T @object) where T : class
    {
        var jsonObject = JsonSerializer.Serialize(@object);
        return JsonSerializer.Deserialize<T>(jsonObject);
    }
}
