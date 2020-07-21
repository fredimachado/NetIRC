using GravyIrc.Attributes;
using System;
using System.Reflection;

namespace GravyIrc.Extensions
{
    internal static class AttributeExtensions
    {
        public static string GetCommand(this Type t) => t.GetCustomAttribute<ServerMessageAttribute>()?.Command;

        public static bool HasCommand(this Type t) => !string.IsNullOrEmpty(t.GetCommand());
    }
}
