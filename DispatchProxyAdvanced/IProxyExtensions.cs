using DispatchProxyAdvanced._internal;
using System;

namespace DispatchProxyAdvanced;


public static class IProxyExtensions
{
    public static Type GetDeclaringType(this IProxy proxy)
    {
        return proxy.GetType().FindProxySourceType()!;
    }

    public static T? GetState<T>(this IProxy proxy)
    {
        if (proxy.CustomProxyStateDefinition != null)
            return (T?)proxy.CustomProxyStateDefinition;

        return default;
    }

    public static void SetState<T>(this IProxy proxy, T? value)
    {
        proxy.CustomProxyStateDefinition = value;
    }

    /// <summary>Represents a thread-safe set (once) state operation.</summary>
    public static T? GetOrAddState<T>(this IProxy proxy, Func<T?> add)
    {
        if (proxy.CustomProxyStateDefinition != null)
            return (T?)proxy.CustomProxyStateDefinition;

        lock (proxy)
        {
            if (proxy.CustomProxyStateDefinition != null)
                return (T?)proxy.CustomProxyStateDefinition;

            var state = add();
            proxy.CustomProxyStateDefinition = state;
            return state;
        }
    }

    /// <summary>Represents a thread-safe update state operation.</summary>
    public static void GetAndUpdateState<T>(this IProxy proxy, Func<T?, T?> getAndUpdate)
    {
        lock (proxy)
        {
            proxy.CustomProxyStateDefinition = getAndUpdate(proxy.GetState<T>());
        }
    }
}
