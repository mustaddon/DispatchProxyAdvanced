namespace Tests;



interface ITest
{
    int Prop1 { get; set; }
    int Method1(int a);
    event EventHandler? Event1;
    void RiseEvent(EventArgs args);
}

interface ITest<T> : ITest
{
    public T Prop2 { get; set; }
    public T Method2(T a);
    public TA Method3<TA, TB>(TA a, TB b);
    public bool Method2(bool a);
}

class TestClass : ITest
{
    public TestClass(int prop1)
    {
        Prop1 = prop1;
    }

    protected int Prop0 { get; set; } = 555;
    public virtual int Prop1 { get; set; }
    public virtual void Method0() { }
    public virtual int Method1(int a) => a;
    public virtual event EventHandler? Event1;
    public virtual void RiseEvent(EventArgs args) { Event1?.Invoke(this, args); }
}

class TestClass<T> : TestClass, ITest<T>
{
    public TestClass(int prop1, T prop2) 
        : base(prop1)
    {
        Prop2 = prop2;
    }

    internal new int Prop0 { get; set; } = 666;

    public virtual T Prop2 { get; set; }

    public virtual T Method2(T a) => a;

    public virtual bool Method2(bool a) => a;

    public virtual TA Method3<TA, TB>(TA a, TB b) => a;

    internal virtual T Method4(T a) => a;

    protected virtual T Method5(T a) => a;
}

abstract class ATest : ITest
{
    public abstract int Prop1 { get; set; }
    public abstract void Method0();
    public abstract int Method1(int a);

    public abstract event EventHandler? Event1;
    public abstract void RiseEvent(EventArgs args);
}

[AttributeUsage(AttributeTargets.All)]
class SomeAttribute(string value) : Attribute
{
    public string Value => value;
}