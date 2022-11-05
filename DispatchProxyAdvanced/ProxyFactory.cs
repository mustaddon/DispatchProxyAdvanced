using DispatchProxyAdvanced.Dynamic;
using System;
using System.Reflection;
namespace DispatchProxyAdvanced;


/// <summary>
/// ProxyFactory provides a mechanism for the instantiation of proxy objects and handling of their method dispatch.
/// </summary>
public static class ProxyFactory
{
    /// <summary>
    /// Creates an object instance that derives from <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The class or interface the proxy should implement.</typeparam>
    /// <param name="handler">Whenever any VIRTUAL method on the generated proxy type is called, this handler will be invoked</param>
    /// <returns>An object instance that derives from <typeparamref name="T"/>.</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is sealed.</exception>
    public static T Create<T>(ProxyHandler handler)
    {
        return (T)Create(typeof(T), handler);
    }

    public static object Create(Type type, ProxyHandler handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        if (type.IsInterface)
            return InterfacedProxy.Create(type, handler);

        if (type.IsSealed)
            throw new ArgumentException($"{type} is sealed");

        var proxyType = ProxyDynamic.Types.GetOrAdd(type, x => new Lazy<Type>(() => DefineProxyType(x))).Value;

        return Activator.CreateInstance(proxyType, handler)!;
    }

    private static Type DefineProxyType(Type type)
    {
        var name = $"generatedProxy_{Guid.NewGuid():N}";

        return ProxyDynamic.Module.DefineType(name, TypeAttributes.Public, type.IsInterface ? null : type)
            .AddGenericParameters(type)
            .AddInterfaces(type)
            .AddFields(out var fields)
            .AddConstructor(type, fields)
            .AddMethods(type, fields)
            .CreateTypeInfo()!;
    }
}
