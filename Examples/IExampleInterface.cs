namespace Examples; 

interface IExampleInterface : IExampleInterfaceB, IExampleInterfaceA
{
    public int MyProp { get; set; }
    public int MyMethod(int a, int b);
}

interface IExampleInterfaceA
{
    public int MyPropA { get; set; }
    public int MyMethodA(int a, int b);
}

interface IExampleInterfaceB
{
    public int MyPropB { get; set; }
    public int MyMethodB(int a, int b);
}


interface IExampleInterface<T1, T2> : IExampleInterfaceA<T2>
{
    public T1 MyProp { get; set; }
    public T1 MyMethod(T1 a, T2 b);
}

interface IExampleInterfaceA<TA>
{
    public TA MyPropA { get; set; }
    public TA MyMethodA(TA a, TA b);
}