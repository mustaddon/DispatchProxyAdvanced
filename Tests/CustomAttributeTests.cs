using DispatchProxyAdvanced._internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection.Emit;

namespace Tests;

public class CustomAttributeTests
{
#if NET5_0_OR_GREATER
    [Test]
    public void TestAttrs_ref()
    {
        var attrs = Enumerable.Range(0, 2).Select(i => $"attr{i}_{Utils.Random.Next()}").ToArray();

        var type = ProxyFactory.CreateType(typeof(ITest<string>),
            new CustomAttributeBuilder(_fromKeyedServicesAttributeCtor, [attrs[0]]),
            new CustomAttributeBuilder(_testAttributeCtor, [attrs[1]]));


        var source = new TestClass<string>(1, attrs[0]);
        var proxy1 = ProxyFactory.Create<ITest<string>>((p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        dynamic proxy2 = type.CreateProxyInstance((p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        var param = type.GetProxyHandlerParameter();

        Assert.That(param?.GetCustomAttribute<FromKeyedServicesAttribute>()?.Key,
            Is.EqualTo(attrs[0]));

        Assert.That(param?.GetCustomAttribute<SomeAttribute>()?.Value,
            Is.EqualTo(attrs[1]));
    }

    static readonly ConstructorInfo _fromKeyedServicesAttributeCtor = typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!;
    static readonly ConstructorInfo _testAttributeCtor = typeof(SomeAttribute).GetConstructor([typeof(string)])!;
#endif
}