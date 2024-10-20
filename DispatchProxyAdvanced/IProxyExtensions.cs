using DispatchProxyAdvanced._internal;
using System;

namespace DispatchProxyAdvanced;


public static class IProxyExtensions
{
    public static Type GetDeclaringType(this IProxy proxy)
    {
        return proxy.GetType().FindProxySourceType()!;
    }

    /// <summary>Represents a thread-safe set (once) state operation.</summary>
    public static object? GetOrAddState(this IProxy proxy, Func<object?> add)
    {
        var state = proxy.CustomProxyStateDefinition;

        if (state == null)
        {
            lock (proxy)
            {
                if (state == null)
                {
                    proxy.CustomProxyStateDefinition = state = add();
                }
            }
        }

        return state;
    }

    /// <summary>Represents a thread-safe update state operation.</summary>
    public static void GetAndUpdateState(this IProxy proxy, Func<object?, object?> getAndUpdate)
    {
        lock (proxy)
        {
            proxy.CustomProxyStateDefinition = getAndUpdate(proxy.CustomProxyStateDefinition);
        }
    }
}
