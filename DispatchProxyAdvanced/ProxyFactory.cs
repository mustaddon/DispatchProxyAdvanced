using DispatchProxyAdvanced._internal;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DispatchProxyAdvanced;


/// <summary>
/// ProxyFactory provides a mechanism for the instantiation of proxy objects and handling of their method dispatch.
/// </summary>
public static class ProxyFactory
{
    /// <summary>
    /// Creates a proxy type that is derives from the source type.
    /// </summary>
    /// <returns>proxy type</returns>
    /// <exception cref="System.ArgumentException">Source type is sealed.</exception>
    public static Type CreateType(Type sourceType, params CustomAttributeBuilder[] handlerParameterAttributes)
    {
        if (sourceType.IsSealed)
            throw new ArgumentException($"'{sourceType}' is sealed.");

        return ProxyDynamic.DefineType(sourceType, handlerParameterAttributes);
    }


    /// <summary>
    /// Creates a proxy instance that is derived from the source type and has the specified method handler.
    /// </summary>
    /// <returns>proxy instance</returns>
    /// <exception cref="System.ArgumentException">Source type is sealed.</exception>
    public static object CreateInstance(Type sourceType, ProxyHandler handler)
    {
        //if (type.IsInterface)
        //    return InterfacedProxy.Create(type, handler);

        return Activator.CreateInstance(CreateType(sourceType), handler)!;
    }


    /// <summary>
    /// Creates a proxy instance that is derived from <typeparamref name="TSource"/> and has the specified method handler.
    /// </summary>
    /// <typeparam name="TSource">The class or interface the proxy should implement.</typeparam>
    /// <param name="handler">Whenever any VIRTUAL method on the generated proxy type is called, this handler will be invoked</param>
    /// <returns>proxy instance</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="TSource"/> is sealed.</exception>
    public static TSource Create<TSource>(ProxyHandler handler)
    {
        return (TSource)CreateInstance(typeof(TSource), handler);
    }


    /// <summary>
    /// Creates a proxy instance that is derived from <typeparamref name="TSource"/> and has the specified method handler.
    /// </summary>
    /// <typeparam name="TSource">The class or interface the proxy should implement.</typeparam>
    /// <param name="handler">Whenever any VIRTUAL method on the generated proxy type is called, this handler will be invoked</param>
    /// <returns>proxy instance</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="TSource"/> is sealed.</exception>
    public static TSource Create<TSource>(Func<MethodInfo, object?[], object?> handler)
    {
        return (TSource)CreateInstance(typeof(TSource), (proxy, method, args) => handler(method, args));
    }


    /// <summary>
    /// Creates a proxy instance that is derived from <typeparamref name="TSource"/> and has the specified method handler.
    /// </summary>
    /// <typeparam name="TSource">The class or interface the proxy should implement.</typeparam>
    /// <param name="instance">source instance</param>
    /// <param name="handler">Whenever any VIRTUAL method on the generated proxy type is called, this handler will be invoked</param>
    /// <returns>proxy instance</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="TSource"/> is sealed.</exception>
    public static TSource CreateSourced<TSource>(TSource instance, Func<TSource, MethodInfo, object?[], object?> handler)
    {
        return (TSource)CreateInstance(typeof(TSource), (proxy, method, args) => handler(instance, method, args));
    }


    /// <summary>
    /// Creates a proxy instance that is derived from <typeparamref name="TSource"/> and has the specified method handler.
    /// </summary>
    /// <typeparam name="TSource">The class or interface the proxy should implement.</typeparam>
    /// <param name="instance">source instance</param>
    /// <param name="handler">Whenever any VIRTUAL method on the generated proxy type is called, this handler will be invoked</param>
    /// <returns>proxy instance</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="TSource"/> is sealed.</exception>
    public static TSource CreateSourced<TSource>(TSource instance, Func<TSource, IProxy, MethodInfo, object?[], object?> handler)
    {
        return (TSource)CreateInstance(typeof(TSource), (proxy, method, args) => handler(instance, proxy, method, args));
    }
}
