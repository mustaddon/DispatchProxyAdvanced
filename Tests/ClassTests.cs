namespace Tests;



public class ClassTests
{
    [Test]
    public void TestGetProp1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<TestClass>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass).GetProperty(nameof(TestClass.Prop1))!.GetGetMethod()));
            return num;
        });

        Assert.That(proxy.Prop1, Is.EqualTo(num));
    }

    [Test]
    public void TestSetProp1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<TestClass>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass).GetProperty(nameof(TestClass.Prop1))!.GetSetMethod()));
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

        var proxy = ProxyFactory.Create<TestClass<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass<string>).GetProperty(nameof(TestClass<string>.Prop2))!.GetGetMethod()));
            Assert.That(args.Length, Is.EqualTo(0));
            return num;
        });

        Assert.That(proxy.Prop2, Is.EqualTo(num));
    }

    [Test]
    public void TestSetProp2_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<TestClass<string>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass<string>).GetProperty(nameof(TestClass<string>.Prop2))!.GetSetMethod()));
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

        var proxy = ProxyFactory.Create<TestClass<object>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass<object>).GetProperty(nameof(TestClass<object>.Prop2))!.GetSetMethod()));
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

        var proxy = ProxyFactory.Create<TestClass>((method, args) =>
        {
            run = true;
            Assert.That(method, Is.EqualTo(typeof(TestClass).GetMethod(nameof(TestClass.Method0))));
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

        var proxy = ProxyFactory.Create<TestClass>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass).GetMethod(nameof(TestClass.Method1))));
            Assert.That(args.Length, Is.EqualTo(1));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method1(num), Is.EqualTo(num));
    }

    [Test]
    public void TestMethod2_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<TestClass<int>>((method, args) =>
        {
            Assert.That(method, Is.EqualTo(typeof(TestClass<int>).GetMethod(nameof(TestClass<int>.Method2))));
            Assert.That(args.Length, Is.EqualTo(1));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method2(num), Is.EqualTo(num));
    }


    [Test]
    public void TestMethod3_val()
    {
        var a = Utils.Random.Next();
        var b = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<TestClass<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(int?), typeof(string) }));
            Assert.That(args, Is.EqualTo(new object?[] { a, b }));
            Assert.That(args.Length, Is.EqualTo(2));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3<int?, string>(a, b), Is.EqualTo(a));
    }


    [Test]
    public void TestMethod3_ref()
    {
        var num = Utils.Random.Next().ToString();

        var proxy = ProxyFactory.Create<TestClass<int>>((method, args) =>
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

        var proxy = ProxyFactory.Create<TestClass<int>>((method, args) =>
        {
            Assert.That(method.GetGenericArguments(), Is.EqualTo(new[] { typeof(object), typeof(object) }));
            Assert.That(args.Length, Is.EqualTo(2));
            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method3(num, num), Is.EqualTo(num));
    }


    [Test]
    public void TestEvent1_add_rise_remove()
    {
        var num = Utils.Random.Next();
        var inst = new TestClass(num);
        var eArgs = new EventArgs();
        var eAdded = false;
        var eRemoved = false;
        var eRised = false;
        var eFired = false;

        EventHandler eHandler = (s, e) =>
        {
            eFired = true;
            Assert.That(s, Is.EqualTo(inst));
            Assert.That(e, Is.EqualTo(eArgs));
        };

        var proxy = ProxyFactory.Create<TestClass>((method, args) =>
        {
            if (method.Name == $"add_{nameof(TestClass.Event1)}")
            {
                eAdded = true;
                Assert.That(args, Is.EqualTo(new[] { eHandler }));
            }
            else if (method.Name == $"remove_{nameof(TestClass.Event1)}")
            {
                eRemoved = true;
                Assert.That(args, Is.EqualTo(new[] { eHandler }));
            }
            else if (method.Name == nameof(TestClass.RiseEvent))
            {
                eRised = true;
                Assert.That(args, Is.EqualTo(new[] { eArgs }));
            }

            return method.Invoke(inst, args);
        });

        Assert.That(eAdded, Is.False);
        Assert.That(eFired, Is.False);

        proxy.Event1 += eHandler;

        Assert.That(eAdded, Is.True);
        Assert.That(eRised, Is.False);
        Assert.That(eFired, Is.False);

        proxy.RiseEvent(eArgs);

        Assert.That(eRised, Is.True);
        Assert.That(eFired, Is.True);
        Assert.That(eRemoved, Is.False);

        eFired = false;

        proxy.Event1 -= eHandler;

        Assert.That(eRemoved, Is.True);
        Assert.That(eFired, Is.False);

        proxy.RiseEvent(eArgs);

        Assert.That(eFired, Is.False);

        //Assert.That(proxy.Method3(num, num), Is.EqualTo(num));
    }
}