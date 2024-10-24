using System;
using System.Reflection;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    private static readonly MethodInfo GetTypeFromHandleMethod = new Func<RuntimeTypeHandle, Type>(Type.GetTypeFromHandle!).Method;
    private static readonly MethodInfo GetMethodInfosMethod = new Func<Type, MethodInfo[]>(ProxyDynamic.ResolveMethods).Method;
    private static readonly MethodInfo MakeGenericMethod = new Func<Type[], MethodInfo>(GetMethodInfosMethod.MakeGenericMethod).Method.GetRuntimeBaseDefinition()!;

    private static readonly Type _voidType = typeof(void);
    private static readonly Type _objectType = typeof(object);
    private static readonly Type _typeType = typeof(Type);
    private static readonly Type _proxyHandlerType = typeof(ProxyHandler);
    private static readonly Type _methodInfoType = typeof(MethodInfo);
    private static readonly Type _methodInfoArrayType = typeof(MethodInfo[]);
}
