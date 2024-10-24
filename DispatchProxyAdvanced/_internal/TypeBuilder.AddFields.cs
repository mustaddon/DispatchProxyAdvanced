using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
    public static TypeBuilder AddFields(this TypeBuilder typeBuilder, out FieldInfo[] fields)
    {
        fields = new FieldInfo[3];

        fields[ProxyFields.Handler] = typeBuilder.DefineField("_handler",
            _proxyHandlerType, FieldAttributes.Private | FieldAttributes.InitOnly);

        fields[ProxyFields.Methods] = typeBuilder.DefineField("_methods",
            _methodInfoArrayType, FieldAttributes.Private | FieldAttributes.InitOnly);

        fields[ProxyFields.State] = typeBuilder.DefineField("_state",
            _objectType, FieldAttributes.Private);

        return typeBuilder;
    }
}
