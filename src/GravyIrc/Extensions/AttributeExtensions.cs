using GravyIrc.Attributes;
using System;
using System.Reflection;

namespace GravyIrc.Extensions
{
    internal static class AttributeExtensions
    {
        public static string GetAction(this Type t) => t.GetCustomAttribute<ServerMessageAttribute>()?.Action;

        public static bool HasAction(this Type t) => !string.IsNullOrEmpty(t.GetAction());
    }
}
