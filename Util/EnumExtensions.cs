using System;
using System.ComponentModel;
using System.Reflection;

namespace Util;

public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        var attribute = (DescriptionAttribute) Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute?.Description ?? value.ToString();
    }
}