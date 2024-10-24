using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddProperties(this TypeBuilder typeBuilder, Type type, out Action<MethodInfo, MethodBuilder> binder)
    {
        if (!type.IsInterface)
        {
            binder = (mi, mb) => { };
            return typeBuilder;
        }

        var map = new Dictionary<MethodInfo, Action<MethodBuilder>>();

        var props = type.GetRuntimeProperties()
            .Concat(type.GetInterfaces()
                .SelectMany(x => x.GetRuntimeProperties()));

        foreach (var pi in props)
        {
            if (pi.GetMethod == null && pi.SetMethod == null)
                continue;

            var builder = typeBuilder.DefineProperty(pi.Name, pi.Attributes, pi.PropertyType, pi.GetIndexParameters().Select(p => p.ParameterType).ToArray());

            if (pi.GetMethod != null)
                map[pi.GetMethod.GetBaseDefinition()] = builder.SetGetMethod;
            if (pi.SetMethod != null)
                map[pi.SetMethod.GetBaseDefinition()] = builder.SetSetMethod;
        }

        binder = (methodInfo, methodBuilder) =>
        {
            if (map.TryGetValue(methodInfo, out var setter))
                setter(methodBuilder);
        };

        return typeBuilder;
    }
}
