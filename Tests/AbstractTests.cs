namespace Tests;

public class AbstractTests
{
    [Test]
    public void TestGetProp1_val()
    {
        var num = Utils.Random.Next();
        
        var proxy = ProxyFactory.Create<ATest>((method, args) =>
        {
            Assert.That(method, 
                Is.EqualTo(typeof(ATest).GetProperty(nameof(ATest.Prop1))!.GetGetMethod()));

            return num;
        });

        Assert.That(proxy.Prop1, Is.EqualTo(num));
    }

    [Test]
    public void TestMethod0_void()
    {
        var num = Utils.Random.Next();
        var run = false;

        var proxy = ProxyFactory.Create<ATest>((method, args) =>
        {
            run = true;

            Assert.That(method, 
                Is.EqualTo(typeof(ATest).GetMethod(nameof(ATest.Method0))));
            
            Assert.That(args.Length, 
                Is.EqualTo(0));

            return null;
        });

        proxy.Method0();

        Assert.That(run, Is.True);
    }

    [Test]
    public void TestMethod1_val()
    {
        var num = Utils.Random.Next();

        var proxy = ProxyFactory.Create<ATest>((method, args) =>
        {
            Assert.That(method, 
                Is.EqualTo(typeof(ATest).GetMethod(nameof(ATest.Method1))));

            return args.FirstOrDefault();
        });

        Assert.That(proxy.Method1(num), 
            Is.EqualTo(num));
    }

}