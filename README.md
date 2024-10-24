# DispatchProxyAdvanced [![NuGet version](https://badge.fury.io/nu/DispatchProxyAdvanced.svg?)](http://badge.fury.io/nu/DispatchProxyAdvanced)
Extended version of DispatchProxy with Class proxying and custom states.


### Example 1: Interface proxing
```C#
interface IExampleInterface
{
    int MyProp { get; set; }
    int MyMethod(int a, int b);
}
```

```C#
using DispatchProxyAdvanced;

var proxy1 = ProxyFactory.Create<IExampleInterface>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy1.MyProp = 222;
proxy1.MyMethod(20, 200);

// Console output: 
// Executing method: set_MyProp, with args: 222
// Executing method: MyMethod, with args: 20, 200
```


### Example 2: Class proxing with source instance
```C#
class ExampleClass
{
    public virtual int MyProp { get; set; }
    public virtual int MyMethod(int a, int b) => a * b;
}
```
```C#
var someInstance = new ExampleClass { MyProp = 111 };

var proxy3 = ProxyFactory.CreateSourced(someInstance, (source, method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return method.Invoke(source, args);
});

Console.WriteLine($"Property value: {proxy3.MyProp}");
Console.WriteLine($"Method result: {proxy3.MyMethod(10, 100)}");

// Console output: 
// Executing method: get_MyProp, with args:
// Property value: 111
// Executing method: MyMethod, with args: 10, 100
// Method result: 1000
```


### Example 3: Custom proxy instance state
```C#
var proxy4 = ProxyFactory.CreateSourced(someInstance, (source, proxy, method, args) =>
{
    // set your custom state to proxy instance
    proxy.CustomProxyStateDefinition = "ExampleStateValue";

    Console.WriteLine($"Executing method: {method.Name}");
    return method.Invoke(source, args);
});

Console.WriteLine($"Start proxy state: '{((IProxy)proxy4).CustomProxyStateDefinition}'");

proxy4.MyMethod(10, 100);

Console.WriteLine($"Current proxy state: '{((IProxy)proxy4).CustomProxyStateDefinition}'");

// Console output: 
// Start proxy state: ''
// Executing method: MyMethod
// Current proxy state: 'ExampleStateValue'
```

[Program.cs](https://github.com/mustaddon/DispatchProxyAdvanced/tree/master/Examples/Program.cs)