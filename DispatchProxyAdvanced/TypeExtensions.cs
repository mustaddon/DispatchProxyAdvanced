using DispatchProxyAdvanced._internal;
using System;
using System.Reflection;

namespace DispatchProxyAdvanced;


public static class TypeExtensions
{
    public static object CreateProxyInstance(this Type proxyType, ProxyHandler proxyHandler)
    {
        return Activator.CreateInstance(proxyType, proxyHandler)!;
    }

    public static bool IsProxyType(this Type type)
    {
        return typeof(IProxy).IsAssignableFrom(type);
    }

    public static Type? GetProxyDeclaringType(this Type type)
    {
        if (!type.IsProxyType())
            return null;

        return type.GetProxySourceType();
    }

    public static ParameterInfo? GetProxyHandlerParameter(this Type type)
    {
        if (!type.IsProxyType())
            return null;

        return type.GetConstructors()[0].GetParameters()[0];
    }
}
