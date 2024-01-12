namespace Tests;

public class InterfaceTests
{
    [Test]
    public void TestGetProp1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<ITest>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest).GetProperty(nameof(ITest.Prop1))!.GetGetMethod()));
            return num;
        });

        Assert.That(proxy.Prop1, Is.EqualTo(num));
    }

    [Test]
    public void TestGetProp2_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<ITest<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest<string>).GetProperty(nameof(ITest<string>.Prop2))!.GetGetMethod()));
            return num;
        });

        Assert.That(proxy.Prop2, Is.EqualTo(num));
    }

    [Test]
    public void TestMethod1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<ITest>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest).GetMethod(nameof(ITest.Method1))));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method1(num), Is.EqualTo(num));
    }

    [Test]
    public void TestMethod2_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(ITest<int>).GetMethod(nameof(ITest<int>.Method2))));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method2(num), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new [] { typeof(int?), typeof(string) }));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3<int?, string>(num, num.ToString()), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<ITest<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(string), typeof(string) }));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3<string, string>(num, num), Is.EqualTo(num));
    }
}