namespace Tests;

public class DynamicTests
{
    [Test]
    public void TestSimpleProps_interface()
    {
        var startval = Utils.Random.Next();
        var source = new TestClass<string>(startval, startval.ToString());

        var proxy1 = ProxyFactory.Create<ITest<string>>((p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        dynamic proxy2 = ProxyFactory.CreateInstance(typeof(ITest<string>), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        var p1 = proxy1.Prop1;
        var p2 = proxy2.Prop1;

        Assert.That(source.Prop1,
            Is.EqualTo(startval));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy1.Prop1));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy2.Prop1));

        var newval = Utils.Random.Next();

        proxy2.Prop1 = newval;

        Assert.That(source.Prop1,
            Is.EqualTo(newval));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy2.Prop1));
    }

    [Test]
    public void TestSimpleProps_class()
    {
        var startval = Utils.Random.Next();
        var source = new TestClass<string>(startval, startval.ToString());

        var proxy1 = ProxyFactory.CreateSourced(source, (s, p, m, a) =>
        {
            return m.Invoke(s, a);
        });

        dynamic proxy2 = ProxyFactory.CreateInstance(source.GetType(), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        var p1 = proxy1.Prop1;
        var p2 = proxy2.Prop1;

        Assert.That(source.Prop1,
            Is.EqualTo(startval));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy1.Prop1));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy2.Prop1));

        var newval = Utils.Random.Next();

        proxy2.Prop1 = newval;

        Assert.That(source.Prop1,
            Is.EqualTo(newval));

        Assert.That(source.Prop1,
            Is.EqualTo(proxy2.Prop1));
    }





    [Test]
    public void TestGenericProps_interface()
    {
        var startval = Utils.Random.Next().ToString();
        var source = new TestClass<string>(Utils.Random.Next(), startval);

        var proxy1 = ProxyFactory.Create<ITest<string>>((p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        dynamic proxy2 = ProxyFactory.CreateInstance(typeof(ITest<string>), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        var p1 = proxy1.Prop2;
        var p2 = proxy2.Prop2;

        Assert.That(source.Prop2,
            Is.EqualTo(startval));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy1.Prop2));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy2.Prop2));

        var newval = Utils.Random.Next().ToString();

        proxy2.Prop2 = newval;

        Assert.That(source.Prop2,
            Is.EqualTo(newval));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy2.Prop2));
    }

    [Test]
    public void TestGenericProps_class()
    {
        var startval = Utils.Random.Next().ToString();
        var source = new TestClass<string>(Utils.Random.Next(), startval);

        var proxy1 = ProxyFactory.CreateSourced(source, (s, p, m, a) =>
        {
            return m.Invoke(s, a);
        });

        dynamic proxy2 = ProxyFactory.CreateInstance(source.GetType(), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        var p1 = proxy1.Prop2;
        var p2 = proxy2.Prop2;

        Assert.That(source.Prop2,
            Is.EqualTo(startval));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy1.Prop2));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy2.Prop2));

        var newval = Utils.Random.Next().ToString();

        proxy2.Prop2 = newval;

        Assert.That(source.Prop2,
            Is.EqualTo(newval));

        Assert.That(source.Prop2,
            Is.EqualTo(proxy2.Prop2));
    }





    [Test]
    public void TestEvents_interface()
    {
        var eArgs = new EventArgs();
        var eFired = false;

        EventHandler eHandler = (s, e) =>
        {
            eFired = true;
        };

        var startval = Utils.Random.Next().ToString();

        var source = new TestClass<string>(Utils.Random.Next(), startval);

        dynamic proxy = ProxyFactory.CreateInstance(typeof(ITest<string>), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        proxy.Event1 += eHandler;
        Assert.That(eFired, Is.False);

        proxy.RiseEvent(eArgs);
        Assert.That(eFired, Is.True);

        eFired = false;

        proxy.Event1 -= eHandler;

        proxy.RiseEvent(eArgs);
        Assert.That(eFired, Is.False);

    }

    [Test]
    public void TestEvents_class()
    {
        var eArgs = new EventArgs();
        var eFired = false;

        EventHandler eHandler = (s, e) =>
        {
            eFired = true;
        };

        var startval = Utils.Random.Next().ToString();

        var source = new TestClass<string>(Utils.Random.Next(), startval);

        dynamic proxy = ProxyFactory.CreateInstance(typeof(ITest<string>), (p, m, a) =>
        {
            return m.Invoke(source, a);
        });

        proxy.Event1 += eHandler;

        Assert.That(eFired, Is.False);
        proxy.RiseEvent(eArgs);
        Assert.That(eFired, Is.True);

        eFired = false;

        proxy.Event1 -= eHandler;

        Assert.That(eFired, Is.False);
        proxy.RiseEvent(eArgs);
        Assert.That(eFired, Is.False);
    }
}