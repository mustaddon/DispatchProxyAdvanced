namespace Examples; 

interface IExampleInterface : IExampleInterfaceB, IExampleInterfaceA
{
    int MyProp { get; set; }
    int MyMethod(int a, int b);
}

interface IExampleInterfaceA
{
    int MyPropA { get; set; }
    int MyMethodA(int a, int b);
}

interface IExampleInterfaceB : IExampleInterfaceC
{
    int MyPropB { get; set; }
    int MyMethodB(int a, int b);
}

interface IExampleInterfaceC {
    int MyPropC { get; set; }
}


interface IExampleInterface<T1, T2> : IExampleInterfaceA<T2>
{
    T1 MyProp { get; set; }
    T1 MyMethod(T1 a, T2 b);
}

interface IExampleInterfaceA<TA>
{
    TA MyPropA { get; set; }
    TA MyMethodA(TA a, TA b);
}