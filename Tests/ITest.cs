using DispatchProxyAdvanced;
using System.Reflection;

namespace Examples;

interface ITest
{
    public int Prop1 { get; set; }
    public int Method1(int a);
}

interface ITest<T> : ITest
{
    public T Prop2 { get; set; }
    public T Method2(T a);
    public TA Method3<TA, TB>(TA a, TB b);
}

public abstract class ATest : ITest
{
    public int Field1 = 555;
    public abstract int Prop1 { get; set; }
    public abstract int Method1(int a);
    public virtual object Method2(string a) => a;
}

class Test
{
    public Test(int prop1)
    {
        Prop1 = prop1;
    }

    public virtual int Prop1 { get; set; }

    public virtual void Method0() { }
    public virtual int Method1(int a) => a;
    public virtual object Method2(string a) => a;
    public virtual string Method3(string a) => a;
}

public class TestA : ATest
{
    public TestA(int prop1)
        : base()
    {
        Prop1 = prop1;
    }

    public override int Prop1 { get; set; }

    public void Method0() { }

    public override int Method1(int a) => a;

    public override object Method2(string a) => a;
}

public class Test<T> : ITest<T>
{
    public Test()
    {
        _handler = (a, b) => b.FirstOrDefault();
        _method = this.GetType().GetMethod(nameof(Method3), BindingFlags.Public | BindingFlags.Instance)!;
    }

    readonly MethodInfo _method;
    readonly ProxyHandler _handler;

    public int Prop1 { get; set; }
    public T Prop2 { get; set; }

    public virtual void Method0() { }

    public virtual int Method1(int a) => a;

    public virtual T Method2(T a) => a;
    public virtual TA Method3<TA, TB>(TA a, TB b)
    {
        var mi = _method.MakeGenericMethod(new[] { typeof(TA), typeof(TB) });
        var result = _handler.Invoke(mi, new object?[] { a, b });
        var rt = mi.ReturnType;
        return (TA)result;
    }
}
