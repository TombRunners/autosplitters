using System.ComponentModel;
using System;
using System.Reflection;

namespace TR4;

public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute?.Description ?? value.ToString();
    }
}