using DispatchProxyAdvanced;
using Examples;


Console.WriteLine("=== Example 1: Interface ===");

var proxy1 = ProxyFactory.Create<IExampleInterface>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy1.MyProp = 222;
proxy1.MyMethod(20, 200);





Console.WriteLine("\n=== Example 2: Abstract class ===");

var proxy2 = ProxyFactory.Create<ExampleAbstractClass>((method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return args.FirstOrDefault();
});

proxy2.MyProp = 333;
proxy2.MyMethod(30, 300);





Console.WriteLine("\n=== Example 3: Simple class ===");

var someInstance = new ExampleClass { MyProp = 111 };

var proxy3 = ProxyFactory.CreateSourced(someInstance, (source, method, args) =>
{
    Console.WriteLine($"Executing method: {method.Name}, with args: {string.Join(", ", args)}");
    return method.Invoke(source, args);
});

Console.WriteLine($"Property value: {proxy3.MyProp}");
Console.WriteLine($"Method result: {proxy3.MyMethod(10, 100)}");





Console.WriteLine("\n=== Example 4: Custom proxy state ===");

var proxy4 = ProxyFactory.CreateSourced(someInstance, (source, proxy, method, args) =>
{
    // set your custom state to proxy instance
    proxy.CustomProxyStateDefinition = "ExampleStateValue";

    Console.WriteLine($"Executing method: {method.Name}");
    return method.Invoke(source, args);
});


Console.WriteLine($"Start proxy state: {((IProxy)proxy4).CustomProxyStateDefinition ?? "null"}");

proxy4.MyMethod(10, 100);

Console.WriteLine($"Current proxy state: {((IProxy)proxy4).CustomProxyStateDefinition}");


Console.WriteLine("\n=== done ===");
