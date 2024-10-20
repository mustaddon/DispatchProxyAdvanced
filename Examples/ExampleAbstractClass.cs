namespace Examples;

abstract class ExampleAbstractClass
{
    public readonly int MyField = 1;
    public abstract int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;
}
