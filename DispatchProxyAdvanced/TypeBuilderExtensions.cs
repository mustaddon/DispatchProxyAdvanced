using DispatchProxyAdvanced.Dynamic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced;

internal static class TypeBuilderExtensions
{
    public static TypeBuilder AddFields(this TypeBuilder typeBuilder, out FieldInfo[] fields)
    {
        fields = new FieldInfo[2];

        fields[(int)ProxyFields.Handler] = typeBuilder.DefineField("_handler",
            _proxyHandlerType, FieldAttributes.Private);

        fields[(int)ProxyFields.Methods] = typeBuilder.DefineField("_methodInfos",
            _methodInfoArrayType, FieldAttributes.Private);

        return typeBuilder;
    }

    public static TypeBuilder AddConstructor(this TypeBuilder typeBuilder, Type type, FieldInfo[] fields)
    {
        var handlerCtor = typeBuilder.DefineConstructor(MethodAttributes.Public,
            CallingConventions.HasThis,
            CtorArgs);

        var handlerCtorIL = handlerCtor.GetILGenerator();

        handlerCtorIL.Emit(OpCodes.Ldarg_0);
        handlerCtorIL.Emit(OpCodes.Ldarg_1);
        handlerCtorIL.Emit(OpCodes.Stfld, fields[(int)ProxyFields.Handler]);

        handlerCtorIL.Emit(OpCodes.Ldarg_0);
        handlerCtorIL.Emit(OpCodes.Ldarg_0);
        handlerCtorIL.Emit(OpCodes.Call, GetTypeMethod);
        handlerCtorIL.Emit(OpCodes.Callvirt, GetBaseTypeMethod);
        handlerCtorIL.Emit(OpCodes.Call, GetMethodInfosMethod);
        handlerCtorIL.Emit(OpCodes.Stfld, fields[(int)ProxyFields.Methods]);

        handlerCtorIL.Emit(OpCodes.Ret);

        return typeBuilder;
    }

    public static TypeBuilder AddMethods(this TypeBuilder typeBuilder, Type type, FieldInfo[] fields)
    {
        var methods = ProxyDynamic.GetMethodInfos(type);
        var handlerInvokeMethod = fields[(int)ProxyFields.Handler].FieldType.GetMethod(nameof(Action.Invoke))!;

        for (var i = 0; i < methods.Length; i++)
        {
            var method = methods[i];

            var parameterTypes = method.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                method.ReturnType,
                parameterTypes
            );

            Type[] genericArguments = Array.Empty<Type>();

            if (method.IsGenericMethod)
                AddGenericParameters(genericArguments = method.GetGenericArguments(), methodBuilder.DefineGenericParameters);

            var methodIL = methodBuilder.GetILGenerator();

            // MethodInfo method;
            methodIL.DeclareLocal(_methodInfoType);

            // method = _methodInfos[i];
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, fields[(int)ProxyFields.Methods]);
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

            // result = _handler(method, new object[2] { a, b });
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, fields[(int)ProxyFields.Handler]);
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



    public static TypeBuilder AddGenericParameters(this TypeBuilder typeBuilder, Type type)
    {
        if (type.IsGenericTypeDefinition)
            AddGenericParameters(type.GetGenericArguments(), typeBuilder.DefineGenericParameters);

        return typeBuilder;
    }

    private static void AddGenericParameters(Type[] genericParameters,
        Func<string[], GenericTypeParameterBuilder[]> defineGenericParameters)
    {
        var genericParametersNames = genericParameters
            .Select(genericType => genericType.Name)
            .ToArray();

        var definedGenericParameters = defineGenericParameters(genericParametersNames);

        for (var i = 0; i < genericParameters.Length; i++)
        {
            var genericParameter = genericParameters[i];
            var definedGenericParameter = definedGenericParameters[i];
            var genericParameterAttributes = genericParameter.GenericParameterAttributes
                                             & ~GenericParameterAttributes.Covariant
                                             & ~GenericParameterAttributes.Contravariant;

            definedGenericParameter.SetGenericParameterAttributes(genericParameterAttributes);

            var genericParameterConstraints = genericParameter.GetGenericParameterConstraints();

            if (!genericParameterConstraints.Any())
                continue;

            var interfaceConstraints = new List<Type>(genericParameterConstraints.Length);

            foreach (var constraint in genericParameterConstraints)
            {
                if (constraint.IsInterface)
                    interfaceConstraints.Add(constraint);
                else
                    definedGenericParameter.SetBaseTypeConstraint(constraint);
            }

            if (interfaceConstraints.Any())
                definedGenericParameter.SetInterfaceConstraints(interfaceConstraints.ToArray());
        }
    }

    public static TypeBuilder AddInterfaces(this TypeBuilder typeBuilder, Type type)
    {
        ProxyDynamic.EnsureTypeIsVisible(type);

        if (type.IsInterface)
            typeBuilder.AddInterfaceImplementation(type);

        foreach (var x in type.GetInterfaces())
            typeBuilder.AddInterfaceImplementation(ProxyDynamic.EnsureTypeIsVisible(x));

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

    private static readonly Type[] CtorArgs = [typeof(ProxyHandler)];
    private static readonly MethodInfo GetTypeMethod = new Func<Type>(string.Empty.GetType).Method;
    private static readonly MethodInfo GetTypeFromHandleMethod = new Func<RuntimeTypeHandle, Type>(Type.GetTypeFromHandle).Method;
    private static readonly MethodInfo GetMethodInfosMethod = new Func<Type, MethodInfo[]>(ProxyDynamic.GetMethodInfos).Method;
    private static readonly MethodInfo MakeGenericMethod = typeof(MethodInfo).GetMethod(nameof(MethodInfo.MakeGenericMethod), BindingFlags.Public | BindingFlags.Instance)!;
    private static readonly MethodInfo GetBaseTypeMethod = typeof(Type).GetProperty(nameof(Type.BaseType), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod(true)!;
    private static readonly MethodInfo GetIsValueTypeMethod = typeof(Type).GetProperty(nameof(Type.IsValueType), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod(true)!;

    private static readonly Type _voidType = typeof(void);
    private static readonly Type _objectType = typeof(object);
    private static readonly Type _typeType = typeof(Type);
    private static readonly Type _proxyHandlerType = typeof(ProxyHandler);
    private static readonly Type _methodInfoType = typeof(MethodInfo);
    private static readonly Type _methodInfoArrayType = typeof(MethodInfo[]);
}
