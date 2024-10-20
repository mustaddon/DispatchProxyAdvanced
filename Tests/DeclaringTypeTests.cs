using System.Threading.Tasks;

namespace Tests;

public class DeclaringTypeTests
{
    [Test]
    public void TestDeclaringType_class()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.CreateSourced(new TestClass(num), (source, pi, method, args) =>
        {
            Assert.That(pi.GetDeclaringType(),
                Is.EqualTo(typeof(TestClass)));

            return method.Invoke(source, args);
        });

        proxy.Method1(111);

        Assert.That(proxy.GetType().GetProxyDeclaringType(),
            Is.EqualTo(typeof(TestClass)));
    }

    [Test]
    public void TestDeclaringType_ref()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.CreateSourced<ITest<string>>(new TestClass<string>(num, num.ToString()), (source, pi, method, args) =>
        {
            Assert.That(pi.GetDeclaringType(),
                Is.EqualTo(typeof(ITest<string>)));

            return method.Invoke(source, args);
        });

        proxy.Method1(111);

        Assert.That(proxy.GetType().GetProxyDeclaringType(),
            Is.EqualTo(typeof(ITest<string>)));
    }


}