using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddMethods(this TypeBuilder typeBuilder, Type type, FieldInfo[] fields, Action<MethodInfo, MethodBuilder>? tryBindToProperty, Action<MethodInfo, MethodBuilder>? tryBindToEvent)
    {
        var methods = type.IsGenericTypeDefinition ? ProxyDynamic.GetMethods(type) : ProxyDynamic.ResolveMethods(type);
        var handlerInvokeMethod = fields[ProxyFields.Handler].FieldType.GetMethod(nameof(Action.Invoke))!;

        for (var i = 0; i < methods.Length; i++)
        {
            var method = methods[i];

            var parameterTypes = method.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                method.ReturnType,
                parameterTypes
            );

            tryBindToProperty?.Invoke(method, methodBuilder);
            tryBindToEvent?.Invoke(method, methodBuilder);

            Type[] genericArguments = [];

            if (method.IsGenericMethod)
                AddGenericParameters(genericArguments = method.GetGenericArguments(), methodBuilder.DefineGenericParameters);

            var methodIL = methodBuilder.GetILGenerator();

            // MethodInfo method;
            methodIL.DeclareLocal(_methodInfoType);

            // method = _methodInfos[i];
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, fields[ProxyFields.Methods]);
            methodIL.Emit(OpCodes.Ldc_I4, i);
            methodIL.Emit(OpCodes.Ldelem_Ref);

            if (method.IsGenericMethod)
            {
                // method = _methodInfos[i].MakeGenericMethod(new object[] { ... });
                methodIL.Emit(OpCodes.Ldc_I4, genericArguments.Length);
                methodIL.Emit(OpCodes.Newarr, _typeType);

                for (var j = 0; j < genericArguments.Length; j++)
                {
                    methodIL.Emit(OpCodes.Dup);
                    methodIL.Emit(OpCodes.Ldc_I4, j);
                    methodIL.Emit(OpCodes.Ldtoken, genericArguments[j]);
                    methodIL.Emit(OpCodes.Call, GetTypeFromHandleMethod);
                    methodIL.Emit(OpCodes.Stelem_Ref);
                }

                methodIL.Emit(OpCodes.Callvirt, MakeGenericMethod);
            }

            methodIL.Emit(OpCodes.Stloc_0);

            // result = _handler(this, method, new object[2] { a, b });
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, fields[ProxyFields.Handler]);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            methodIL.Emit(OpCodes.Newarr, _objectType);

            for (var j = 0; j < parameterTypes.Length; j++)
            {
                var parameterType = parameterTypes[j];

                methodIL.Emit(OpCodes.Dup);
                methodIL.Emit(OpCodes.Ldc_I4, j);
                methodIL.Emit(OpCodes.Ldarg, j + 1);

                if (parameterType.IsValueType || parameterType.IsGenericParameter)
                    methodIL.Emit(OpCodes.Box, parameterType);

                methodIL.Emit(OpCodes.Stelem_Ref);
            }
            methodIL.Emit(OpCodes.Callvirt, handlerInvokeMethod);

            if (method.ReturnType == _voidType)
            {
                // return;
                methodIL.Emit(OpCodes.Pop);
            }
            else
            {
                // return (TResult)result;
                if (method.ReturnType.IsValueType || method.ReturnType.IsGenericParameter)
                    methodIL.Emit(OpCodes.Unbox_Any, method.ReturnType);
                else if (method.ReturnType != _objectType)
                    methodIL.Emit(OpCodes.Castclass, method.ReturnType);
            }
            methodIL.Emit(OpCodes.Ret);
        }

        return typeBuilder;
    }


    //private static MethodInfo ForceVirtual(this MethodInfo method)
    //{
    //    if (!method.IsVirtual && !method.IsAbstract)
    //    {
    //        var field = method.GetType().GetField("m_methodAttributes", BindingFlags.NonPublic | BindingFlags.Instance)!;
    //        var value = (MethodAttributes)field.GetValue(method)!;
    //        value |= MethodAttributes.Virtual;
    //        field.SetValue(method, value);
    //    }
    //    return method;
    //}
}
