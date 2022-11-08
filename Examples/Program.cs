using DispatchProxyAdvanced;
using Examples;


Console.WriteLine("=== Example 1 ===");

var instanceTarget = new ExampleClass { MyProp = 111 };

var proxy1 = ProxyFactory.Create<ExampleClass>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return method.Invoke(instanceTarget, args);
});

Console.WriteLine($"Property value: {proxy1.MyProp}");
Console.WriteLine($"Method result: {proxy1.MyMethod(10, 100)}");



Console.WriteLine("\n=== Example 2 ===");

var proxy2 = ProxyFactory.Create<IExampleInterface>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy2.MyProp = 222;
proxy2.MyMethod(20, 200);



Console.WriteLine("\n=== Example 3 ===");

var proxy3 = ProxyFactory.Create<ExampleAbstractClass>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy3.MyProp = 333;
proxy3.MyMethod(30, 300);



Console.WriteLine("\n=== done ===");
