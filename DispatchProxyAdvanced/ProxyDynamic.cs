using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced.Dynamic;

public static class ProxyDynamic
{
    public static MethodInfo[] GetMethodInfos(Type type)
    {
        return _typeMethods.GetOrAdd(type, x => new Lazy<MethodInfo[]>(() => GetMethods(x))).Value;
    }

    private static MethodInfo[] GetMethods(Type type)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        var methods = type.GetMethods(flags).AsEnumerable();

        if (type.IsInterface)
            methods = methods.Concat(type.GetInterfaces().SelectMany(x => x.GetMethods(flags)));

        return methods.Where(x => x.IsVirtual && !x.IsFinal).Distinct().ToArray();
    }

    internal static Type EnsureTypeIsVisible(this Type type)
    {
        if (!type.IsVisible)
        {
            var assemblyName = type.Assembly.GetName().Name!;

            _ignoresAccessAssemblyNames.GetOrAdd(assemblyName, x =>
            {
                var customAttributeBuilder = new CustomAttributeBuilder(_ignoresAccessChecksAttributeConstructor.Value, new object[] { assemblyName });
                Assembly.SetCustomAttribute(customAttributeBuilder);
                return true;
            });
        }
        return type;
    }


    private const string AssemblyName = "DispatchProxyAdvanced.Dynamic";

    internal static readonly AssemblyBuilder Assembly = AssemblyBuilder
        .DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);

    internal static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(AssemblyName);

    internal static readonly ConcurrentDictionary<Type, Lazy<Type>> Types = new();
    private static readonly ConcurrentDictionary<Type, Lazy<MethodInfo[]>> _typeMethods = new();
    private static readonly ConcurrentDictionary<string, bool> _ignoresAccessAssemblyNames = new();
    private static readonly Lazy<ConstructorInfo> _ignoresAccessChecksAttributeConstructor = new(() => IgnoreAccessChecksToAttributeBuilder.AddToModule(Module));
}
