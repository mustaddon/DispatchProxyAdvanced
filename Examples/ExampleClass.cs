using DispatchProxyAdvanced;
using DispatchProxyAdvanced._internal;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Examples;

class ExampleClass
{
    public virtual int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;



    object? _value;
    public object? Value
    {
        get => _value;
        set => _value = value;
    }
}


class ExampleClass111<T1, T2>
{
    public ExampleClass111([FromKeyedServices("TEST")] Type type)
    {
        _type = type;

        _mm = ProxyDynamic.ResolveMethods(typeof(ExampleClass111<T1, T2>));

        _type2 = typeof(ExampleClass111<T1, T2>);
    }

    readonly Type _type;

    readonly Type _type2;

    readonly Type _type3;

    readonly MethodInfo[] _mm;

    ProxyHandler _handler;

    public void Method(ProxyHandler a)
    {
        lock (this)
        {
            _handler = a;
        }
    }

    readonly object _lock = new();

    public object? CustomProxyStateDefinition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Method2(ProxyHandler a)
    {
        lock (_lock)
        {
            _handler = a;
        }
    }
}
