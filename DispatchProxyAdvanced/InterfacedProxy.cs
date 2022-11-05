using System;
using System.Reflection;
namespace DispatchProxyAdvanced;

internal class InterfacedProxy : DispatchProxy
{
    public static object Create(Type type, ProxyHandler handler)
    {
        var proxy = (InterfacedProxy)CreateMethod
            .MakeGenericMethod(type, typeof(InterfacedProxy))
            .Invoke(null, Array.Empty<object>())!;

        proxy._handler = handler;
        return proxy;
    }

    private ProxyHandler? _handler;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) 
        => _handler!.Invoke(targetMethod!, args!)!;

    private static readonly MethodInfo CreateMethod = typeof(DispatchProxy)
        .GetMethod(nameof(DispatchProxy.Create), BindingFlags.Public | BindingFlags.Static)!;
}
