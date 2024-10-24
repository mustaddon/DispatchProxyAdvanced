using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DispatchProxyAdvanced._internal;

public static class ProxyDynamic
{
    internal static Type CreateType(Type sourceType, params CustomAttributeBuilder[] handlerAttributes)
    {
        if (handlerAttributes.Length > 0)
            return DefineType(sourceType, handlerAttributes);

        return ResolveType(sourceType);
    }

    static Type ResolveType(Type sourceType)
    {
        return _definedTypes.GetOrAdd(sourceType, static t => new Lazy<Type>(() =>
        {
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                return ResolveType(t.GetGenericTypeDefinition())
                    .MakeGenericType(t.GenericTypeArguments);

            return DefineType(t);
        })).Value;
    }

    static TypeInfo DefineType(Type type, params CustomAttributeBuilder[] handlerAttributes)
    {
        return Module
            .DefineType($"generatedProxy_{Guid.NewGuid():N}", TypeAttributes.Public, type.IsInterface ? null : type)
            .AddGenericParameters(type)
            .AddInterfaces(type)
            .AddFields(out var fields)
            .AddConstructor(type, fields, handlerAttributes)
            .AddProperties(type, out var propertyBinder)
            .AddEvents(type, out var eventBinder)
            .AddMethods(type, fields, propertyBinder, eventBinder)
            .AddProxyAddons(fields)
            .CreateTypeInfo()!;
    }

    public static MethodInfo[] ResolveMethods(Type type)
    {
        return _typeMethods.GetOrAdd(type, static x => new Lazy<MethodInfo[]>(() => GetMethods(x))).Value;
    }

    internal static MethodInfo[] GetMethods(Type type)
    {
        var methods = type.GetRuntimeMethods();

        if (type.IsInterface)
            methods = methods
                .Concat(type.GetInterfaces().SelectMany(x => x.GetRuntimeMethods()))
                .Concat(typeof(object).GetRuntimeMethods());

        return [.. methods
            .Where(x => x.IsVirtual && !x.IsFinal && (x.IsPublic || x.IsAssembly))
            .Select(x => x.GetBaseDefinition())
            .OrderBy(x => x.Name)
            .ThenBy(x => x.GetGenericArguments().Length)
            .ThenBy(x => x.GetParameters().Length)];
    }

    internal static Type EnsureTypeIsVisible(this Type type)
    {
        if (!type.IsVisible)
        {
            var assemblyName = type.Assembly.GetName().Name!;

            _ignoresAccessAssemblyNames.GetOrAdd(assemblyName, x =>
            {
                var customAttributeBuilder = new CustomAttributeBuilder(_ignoresAccessChecksAttributeConstructor.Value, [assemblyName]);
                Assembly.SetCustomAttribute(customAttributeBuilder);
                return true;
            });
        }
        return type;
    }


    private const string AssemblyName = "DispatchProxyAdvanced.Dynamic";

    private static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);

    private static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(AssemblyName);

    private static readonly ConcurrentDictionary<Type, Lazy<Type>> _definedTypes = new();

    private static readonly ConcurrentDictionary<Type, Lazy<MethodInfo[]>> _typeMethods = new();

    private static readonly ConcurrentDictionary<string, bool> _ignoresAccessAssemblyNames = new();

    private static readonly Lazy<ConstructorInfo> _ignoresAccessChecksAttributeConstructor = new(() => IgnoreAccessChecksToAttributeBuilder.AddToModule(Module));
}
