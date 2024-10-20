﻿using System;
using System.Linq;

namespace DispatchProxyAdvanced._internal;


internal static class TypeExtensions
{
    internal static Type? FindProxySourceType(this Type type)
    {
        if (type.BaseType != typeof(object))
            return type.BaseType!;

        return type.GetInterfaces().FirstOrDefault()!;
    }
}
