namespace Examples;

abstract class ExampleAbstractClass
{
    public abstract int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;
}
