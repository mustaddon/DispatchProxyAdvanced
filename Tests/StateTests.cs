using System.Threading.Tasks;

namespace Tests;

public class StateTests
{
    [Test]
    public void TestSetState_class()
    {
        var num = Utils.Random.Next();
        var source = new TestClass(num);

        var proxy = ProxyFactory.Create<TestClass>((pi, method, args) =>
        {
            Assert.That(pi.CustomProxyStateDefinition,
                Is.Null);

            pi.CustomProxyStateDefinition = num;

            Assert.That(pi.CustomProxyStateDefinition,
                Is.EqualTo(num));

            return method.Invoke(source, args);
        });

        Assert.That(((IProxy)proxy).CustomProxyStateDefinition,
            Is.Null);

        Assert.That(proxy.Prop1,
            Is.EqualTo(source.Prop1));

        Assert.That(((IProxy)proxy).CustomProxyStateDefinition,
            Is.EqualTo(num));
    }


    [Test]
    public void TestSetState_i()
    {
        var num = Utils.Random.Next();
        var source = new TestClass(num);

        var proxy = ProxyFactory.Create<ITest>((pi, method, args) =>
        {
            Assert.That(pi.CustomProxyStateDefinition,
                Is.Null);

            Assert.That(pi.GetOrAddState(() => num),
                Is.EqualTo(num));

            Assert.That(pi.GetOrAddState(() => num*2),
                Is.EqualTo(num));

            return method.Invoke(source, args);
        });

        Assert.That(((IProxy)proxy).CustomProxyStateDefinition,
            Is.Null);

        Assert.That(proxy.Prop1,
            Is.EqualTo(source.Prop1));

        Assert.That(((IProxy)proxy).CustomProxyStateDefinition,
            Is.EqualTo(num));
    }



    [Test]
    public void TestSetState_lock()
    {
        var num = Utils.Random.Next();
        var source = new TestClass(num);

        var proxy = ProxyFactory.Create<ITest>((pi, method, args) =>
        {
            pi.CustomProxyStateDefinition = 0;
            var count = 1000;

            Task.WaitAll(Enumerable.Range(0, count)
                .Select(i => Task.Run(() => pi.GetAndUpdateState(state => (int)state! + 1)))
                .ToArray());

            Assert.That(pi.CustomProxyStateDefinition,
                Is.EqualTo(count));

            return method.Invoke(source, args);
        });

        Assert.That(proxy.Prop1,
            Is.EqualTo(source.Prop1));
    }


}