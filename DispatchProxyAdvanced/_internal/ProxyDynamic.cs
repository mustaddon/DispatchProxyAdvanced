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
        {
            return DefineType(sourceType, handlerAttributes);
        }

        if (sourceType.IsGenericType && !sourceType.IsGenericTypeDefinition)
        {
            return ResolveType(sourceType.GetGenericTypeDefinition())
                .MakeGenericType(sourceType.GenericTypeArguments);
        }

        return ResolveType(sourceType);
    }

    static Type DefineType(Type type, params CustomAttributeBuilder[] handlerAttributes)
    {
        return Module
            .DefineType($"generatedProxy_{Guid.NewGuid():N}", TypeAttributes.Public, type.IsInterface ? null : type)
            .AddGenericParameters(type)
            .AddInterfaces(type)
            .AddFields(out var fields)
            .AddConstructor(type, fields, handlerAttributes)
            .AddSourceMethods(type, fields)
            .AddProxyMethods(fields)
            .CreateTypeInfo()!;
    }

    static Type ResolveType(Type sourceType)
    {
        return _definedTypes.GetOrAdd(sourceType, type => new Lazy<Type>(() => DefineType(type))).Value;
    }

    public static MethodInfo[] ResolveMethods(Type type)
    {
        return _typeMethods.GetOrAdd(type, static x => new Lazy<MethodInfo[]>(() => GetMethods(x))).Value;
    }

    internal static MethodInfo[] GetMethods(Type type)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        var methods = type.GetMethods(flags).AsEnumerable();

        if (type.IsInterface)
            methods = methods
                .Concat(type.GetInterfaces().SelectMany(x => x.GetMethods(flags)))
                .Concat(typeof(object).GetMethods(flags));

        return [.. methods
            .Where(x => x.IsVirtual && !x.IsFinal)
            .Select(x => x.GetBaseDefinition())
            .Distinct()];
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
