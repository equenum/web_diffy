using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebPageChangeMonitor.Api.Infrastructure.Schemas;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Description = GetPossibleEnumValues(context.Type);
        }
    }

    private static string GetPossibleEnumValues(Type enumType)
    {
        var values = Enum.GetValues(enumType)
            .Cast<object>()
            .Select(value => $"{(int)value}-{value}");

        return string.Join(", ", values);
    }
}
