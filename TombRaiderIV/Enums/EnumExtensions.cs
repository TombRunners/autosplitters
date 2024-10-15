using System.ComponentModel;
using System;

namespace TR4;

public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute == null ? value.ToString() : attribute.Description;
    }
}