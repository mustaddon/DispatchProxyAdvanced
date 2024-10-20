# DispatchProxyAdvanced [![NuGet version](https://badge.fury.io/nu/DispatchProxyAdvanced.svg?)](http://badge.fury.io/nu/DispatchProxyAdvanced)
Extended version of DispatchProxy with Class proxying.


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

var someInstance = new ExampleClass { MyProp = 111 };

var proxy1 = ProxyFactory.CreateSourced(someInstance, (source, method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return method.Invoke(source, args);
});

Console.WriteLine($"Property value: {proxy1.MyProp}");
Console.WriteLine($"Method result: {proxy1.MyMethod(10, 100)}");

// Console output: 
// Executing method: get_MyProp, with args:
// Property value: 111
// Executing method: MyMethod, with args: 10, 100
// Method result: 1000
```

## Example 2: Interface proxing
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
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy2.MyProp = 222;
proxy2.MyMethod(20, 200);

// Console output: 
// Executing method: set_MyProp, with args: 222
// Executing method: MyMethod, with args: 20, 200
```

[Program.cs](https://github.com/mustaddon/DispatchProxyAdvanced/tree/master/Examples/Program.cs)