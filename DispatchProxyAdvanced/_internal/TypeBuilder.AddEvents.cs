using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddEvents(this TypeBuilder typeBuilder, Type type, out Action<MethodInfo, MethodBuilder> binder)
    {
        if (!type.IsInterface)
        {
            binder = (mi, mb) => { };
            return typeBuilder;
        }

        var map = new Dictionary<MethodInfo, Action<MethodBuilder>>();

        var events = type.GetRuntimeEvents()
            .Concat(type.GetInterfaces()
                .SelectMany(x => x.GetRuntimeEvents()));

        foreach (var ei in events)
        {
            if (ei.AddMethod == null && ei.RemoveMethod == null && ei.RaiseMethod == null)
                continue;

            var builder = typeBuilder.DefineEvent(ei.Name, ei.Attributes, ei.EventHandlerType!);

            if (ei.AddMethod != null)
                map[ei.AddMethod.GetBaseDefinition()] = builder.SetAddOnMethod;
            if (ei.RemoveMethod != null)
                map[ei.RemoveMethod.GetBaseDefinition()] = builder.SetRemoveOnMethod;
            if (ei.RaiseMethod != null)
                map[ei.RaiseMethod.GetBaseDefinition()] = builder.SetRaiseMethod;
        }

        binder = (methodInfo, methodBuilder) =>
        {
            if (map.TryGetValue(methodInfo, out var setter))
                setter(methodBuilder);
        };

        return typeBuilder;
    }
}
