using System;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddConstructor(this TypeBuilder typeBuilder, Type type, FieldInfo[] fields, CustomAttributeBuilder[] handlerAttributes)
    {
        var handlerCtor = typeBuilder.DefineConstructor(MethodAttributes.Public,
            CallingConventions.HasThis,
            CtorArgs);

        var paramBuilder = handlerCtor.DefineParameter(1, ParameterAttributes.None, "handler");
        foreach (var attrBuilder in handlerAttributes)
            paramBuilder.SetCustomAttribute(attrBuilder);

        var handlerCtorIL = handlerCtor.GetILGenerator();

        handlerCtorIL.Emit(OpCodes.Ldarg_0);
        handlerCtorIL.Emit(OpCodes.Ldarg_1);
        handlerCtorIL.Emit(OpCodes.Stfld, fields[ProxyFields.Handler]);

        var sourceType = !type.IsGenericTypeDefinition ? type : type.MakeGenericType(typeBuilder.GetGenericTypeDefinition().GetGenericArguments());

        handlerCtorIL.Emit(OpCodes.Ldarg_0);
        handlerCtorIL.Emit(OpCodes.Ldtoken, sourceType);
        handlerCtorIL.Emit(OpCodes.Call, GetTypeFromHandleMethod);
        handlerCtorIL.Emit(OpCodes.Call, GetMethodInfosMethod);
        handlerCtorIL.Emit(OpCodes.Stfld, fields[ProxyFields.Methods]);

        handlerCtorIL.Emit(OpCodes.Ret);

        return typeBuilder;
    }


    private static readonly Type[] CtorArgs = [typeof(ProxyHandler)];
}
