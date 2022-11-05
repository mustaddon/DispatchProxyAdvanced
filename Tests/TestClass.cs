namespace Tests;



public class TestClass
{
    [Test]
    public void TestGetProp1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test).GetProperty(nameof(Test.Prop1))!.GetGetMethod()));
            return num;
        });

        Assert.That(proxy.Prop1, Is.EqualTo(num));
    }

    [Test]
    public void TestSetProp1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test).GetProperty(nameof(Test.Prop1))!.GetSetMethod()));
            Assert.That(args.Length, Is.EqualTo(1));
            Assert.That(args.FirstOrDefault(), Is.EqualTo(num));
            return args.FirstOrDefault();
        });

        proxy.Prop1 = num;
    }

    [Test]
    public void TestGetProp2_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<Test<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test<string>).GetProperty(nameof(Test<string>.Prop2))!.GetGetMethod()));
            Assert.That(args.Length, Is.EqualTo(0));
            return num;
        });

        Assert.That(proxy.Prop2, Is.EqualTo(num));
    }

    [Test]
    public void TestSetProp2_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<Test<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test<string>).GetProperty(nameof(Test<string>.Prop2))!.GetSetMethod()));
            Assert.That(args.Length, Is.EqualTo(1));
            Assert.That(args.FirstOrDefault(), Is.EqualTo(num));
            return args.FirstOrDefault();
        });

        proxy.Prop2 = num;
    }

    [Test]
    public void TestSetProp2_obj()
    {
        var num = (object)Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test<object>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test<object>).GetProperty(nameof(Test<object>.Prop2))!.GetSetMethod()));
            Assert.That(args.Length, Is.EqualTo(1));
            Assert.That(args.FirstOrDefault(), Is.EqualTo(num));
            return args.FirstOrDefault();
        });

        proxy.Prop2 = num;
    }

    [Test]
    public void TestMethod0_void()
    {
        var num = Utils.Random.Next();
        var run = false;

        var proxy = ProxyFactory.Create<Test>((method, args) =>
        {
            run = true;
            Assert.That(method, Is.EqualTo(typeof(Test).GetMethod(nameof(Test.Method0))));
            Assert.That(args.Length, Is.EqualTo(0));
            return null;
        });

        proxy.Method0();

        Assert.That(run, Is.True);
    }

    [Test]
    public void TestMethod1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test).GetMethod(nameof(Test.Method1))));
            Assert.That(args.Length, Is.EqualTo(1));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method1(num), Is.EqualTo(num));
    }

    [Test]
    public void TestMethod2_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test<int>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(Test<int>).GetMethod(nameof(Test<int>.Method2))));
            Assert.That(args.Length, Is.EqualTo(1));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method2(num), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(int?), typeof(string) }));
            Assert.That(args.Length, Is.EqualTo(2));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3<int?, string>(num, num.ToString()), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<Test<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(string), typeof(string) }));
            Assert.That(args.Length, Is.EqualTo(2));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3(num, num), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_obj()
    {
        var num = (object)Utils.Random.Next();

        var proxy = ProxyFactory.Create<Test<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(object), typeof(object) }));
            Assert.That(args.Length, Is.EqualTo(2));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3(num, num), Is.EqualTo(num));
    }
}