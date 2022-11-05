# DispatchProxyAdvanced [![NuGet version](https://badge.fury.io/nu/DispatchProxyAdvanced.svg?)](http://badge.fury.io/nu/DispatchProxyAdvanced)
Extended version of DispatchProxy with Class proxying


## Example 1: Class proxing with target
```C#
class ExampleClass
{
    public virtual int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;
}
```
```C#
using DispatchProxyAdvanced;

var instanceTarget = new ExampleClass { MyProp = 111 };

var proxy1 = ProxyFactory.Create<ExampleClass>((method, args) =>
{
    var result = method.Invoke(instanceTarget, args);
    Console.WriteLine($"Invoke: {method.Name}, Args: {string.Join(", ", args)}, Result: {result}");
    return result;
});

Console.WriteLine($"PropValue: {proxy1.MyProp}");
Console.WriteLine($"MethodResult: {proxy1.MyMethod(10, 100)}");

// Console output: 
// Invoke: get_MyProp, Args: , Result: 111
// PropValue: 111
// Invoke: MyMethod, Args: 10, 100, Result: 1000
// MethodResult: 1000
```

## Example 2: Interface proxing targetless
```C#
interface IExampleInterface
{
    public int MyProp { get; set; }
    public int MyMethod(int a, int b);
}
```

```C#
var proxy2 = ProxyFactory.Create<IExampleInterface>((method, args) =>
{
    Console.WriteLine($"Invoke: {method.Name}, Args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy2.MyProp = 222;
proxy2.MyMethod(20, 200);

// Console output: 
// Invoke: set_MyProp, Args: 222
// Invoke: MyMethod, Args: 20, 200
```

## Example 3: Abstract Class proxing targetless
```C#
abstract class ExampleAbstractClass
{
    public abstract int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;
}
```

```C#
var proxy3 = ProxyFactory.Create<ExampleAbstractClass>((method, args) =>
{
    Console.WriteLine($"Invoke: {method.Name}, Args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy3.MyProp = 333;
proxy3.MyMethod(30, 300);

// Console output: 
// Invoke: set_MyProp, Args: 333
// Invoke: MyMethod, Args: 30, 300
```