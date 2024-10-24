using System;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddInterfaces(this TypeBuilder typeBuilder, Type type)
    {
        type.EnsureTypeIsVisible();

        if (type.IsInterface)
            typeBuilder.AddInterfaceImplementation(type);

        foreach (var x in type.GetInterfaces())
            typeBuilder.AddInterfaceImplementation(x.EnsureTypeIsVisible());

        return typeBuilder;
    }
}
