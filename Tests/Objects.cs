namespace Tests;



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
    public bool Method2(bool a);
}

class TestClass : ITest
{
    public TestClass(int prop1)
    {
        Prop1 = prop1;
    }

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

    public virtual T Prop2 { get; set; }

    public virtual T Method2(T a) => a;

    public virtual bool Method2(bool a) => a;

    public virtual TA Method3<TA, TB>(TA a, TB b) => a;
}

abstract class ATest : ITest
{
    public abstract int Prop1 { get; set; }
    public abstract void Method0();
    public abstract int Method1(int a);
}

[AttributeUsage(AttributeTargets.All)]
class SomeAttribute(string value) : Attribute
{
    public string Value => value;
}