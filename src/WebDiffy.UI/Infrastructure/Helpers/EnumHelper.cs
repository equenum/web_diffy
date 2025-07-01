using System;

namespace WebDiffy.UI.Infrastructure.Helpers;

public class EnumHelper
{
    public static TEnum GetEnumValue<TEnum>(string value) where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(value);
    }
}
