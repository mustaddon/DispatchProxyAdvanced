using System;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddProxyAddons(this TypeBuilder typeBuilder, FieldInfo[] fields)
    {
        typeBuilder.AddInterfaceImplementation(typeof(IProxy));

        var attrs = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.ReuseSlot;
        var stateField = fields[ProxyFields.State];

        // State getter
        var getMethod = typeBuilder.DefineMethod(_proxyStateProp.GetGetMethod()!.Name, attrs, stateField.FieldType, Type.EmptyTypes);
        var getMethodIL = getMethod.GetILGenerator();
        getMethodIL.Emit(OpCodes.Ldarg_0);
        getMethodIL.Emit(OpCodes.Ldfld, stateField);
        getMethodIL.Emit(OpCodes.Ret);

        // State setter
        var setMethod = typeBuilder.DefineMethod(_proxyStateProp.GetSetMethod()!.Name, attrs, null, [stateField.FieldType]);
        var setMethodIL = setMethod.GetILGenerator();
        setMethodIL.Emit(OpCodes.Ldarg_0);
        setMethodIL.Emit(OpCodes.Ldarg_1);
        setMethodIL.Emit(OpCodes.Stfld, stateField);
        setMethodIL.Emit(OpCodes.Ret);

        return typeBuilder;
    }


    private static readonly PropertyInfo _proxyStateProp = typeof(IProxy).GetProperty(nameof(IProxy.CustomProxyStateDefinition))!;
}
