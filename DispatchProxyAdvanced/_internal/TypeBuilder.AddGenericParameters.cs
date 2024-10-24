using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DispatchProxyAdvanced._internal;

internal static partial class TypeBuilderExtensions
{
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

}
