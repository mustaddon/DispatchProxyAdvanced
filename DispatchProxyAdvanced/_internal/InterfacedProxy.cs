//using System;
//using System.Reflection;
//namespace DispatchProxyAdvanced._internal;


//internal delegate object? InterfacedProxyHandler(MethodInfo method, object?[] args);

//internal class InterfacedProxy : DispatchProxy
//{
//    public static object Create(Type type, InterfacedProxyHandler handler)
//    {
//        var proxy = (InterfacedProxy)CreateMethod
//            .MakeGenericMethod(type, _interfacedProxy)
//            .Invoke(null, null)!;

//        proxy._handler = handler;
//        return proxy;
//    }

//    private InterfacedProxyHandler? _handler;

//    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
//        => _handler!.Invoke(targetMethod!, args!)!;

//    private static readonly MethodInfo CreateMethod = new Func<object>(Create<object, InterfacedProxy>)
//        .Method.GetGenericMethodDefinition();

//    private static readonly Type _interfacedProxy = typeof(InterfacedProxy);
//}
