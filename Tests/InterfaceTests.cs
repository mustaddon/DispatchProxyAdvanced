namespace Tests;

public class InterfaceTests
{
    [Test]
    public void TestGetProp1_val()
    {
        var num = Utils.Random.Next();
        var source = new TestClass(num);

        var proxy = ProxyFactory.Create<ITest>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest).GetProperty(nameof(ITest.Prop1))!.GetGetMethod()));
            return method.Invoke(source, args);
        });

        Assert.That(proxy.Prop1, 
            Is.EqualTo(source.Prop1));
    }

    [Test]
    public void TestGetProp2_ref()
    {
        var num = Utils.Random.Next();
        var source = new TestClass<string>(num, num.ToString());

        var proxy = ProxyFactory.Create<ITest<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest<string>).GetProperty(nameof(ITest<string>.Prop2))!.GetGetMethod()));
            return method.Invoke(source, args);
        });

        Assert.That(proxy.Prop2, 
            Is.EqualTo(source.Prop2));
    }

    [Test]
    public void TestMethod1_val()
    {
        var num = Utils.Random.Next();
        var source = new TestClass(num);

        var proxy = ProxyFactory.Create<ITest>((method, args) =>
        {
            Assert.That(method, 
                Is.EqualTo(typeof(ITest).GetMethod(nameof(ITest.Method1))));

            return method.Invoke(source, args);
        });

        Assert.That(proxy.Method1(num), 
            Is.EqualTo(source.Method1(num)));
    }

    [Test]
    public void TestMethod2_val()
    {
        var num = Utils.Random.Next();
        var source = new TestClass<int>(num, num - 1);

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method, 
                Is.EqualTo(typeof(ITest<int>).GetMethod(nameof(ITest<int>.Method2))));

            return method.Invoke(source, args);
        });

        Assert.That(proxy.Method2(num), 
            Is.EqualTo(source.Method2(num)));
    }


    [Test]
    public void TestMethod3_val()
    {
        var num = Utils.Random.Next();
        var source = new TestClass<int>(num, num - 1);

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), 
                Is.EqualTo(new [] { typeof(int?), typeof(string) }));

            Assert.That(method,
                Is.EqualTo(typeof(ITest<int>)
                    .GetMethod(nameof(ITest<int>.Method3))!
                    .MakeGenericMethod(typeof(int?), typeof(string))));

            return method.Invoke(source, args);
        });

        Assert.That(proxy.Method3<int?, string>(num, num.ToString()), 
            Is.EqualTo(source.Method3<int?, string>(num, num.ToString())));
    }


    [Test]
    public void TestMethod3_ref()
    {
        var num = Utils.Random.Next();
        var source = new TestClass<int>(num, num - 1);

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), 
                Is.EqualTo(new[] { typeof(string), typeof(string) }));

            Assert.That(method,
                Is.EqualTo(typeof(ITest<int>)
                    .GetMethod(nameof(ITest<int>.Method3))!
                    .MakeGenericMethod(typeof(string), typeof(string))));

            return method.Invoke(source, args);
        });

        Assert.That(proxy.Method3(num.ToString(), num.ToString()), 
            Is.EqualTo(source.Method3(num.ToString(), num.ToString())));
    }
}