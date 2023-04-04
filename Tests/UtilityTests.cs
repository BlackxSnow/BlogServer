using System.Globalization;
using Utility.Arguments;

namespace Tests;

public class UtilityTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Argument("noparams")]
    [Argument("n")]
    private static void Arg_NoParams()
    {
        _ArgNoParamsValue = 1;
    }

    private static int _ArgNoParamsValue = 0;
    [Test]
    public void Argument_NoParameters()
    {
        _ArgNoParamsValue = 0;
        ArgumentParser.Parse(new []{"--noparams"});
        Assert.That(_ArgNoParamsValue, Is.EqualTo(1));
        
    }
    
    [Argument("strings")]
    private static void ArgWithStrings(string a, string b)
    {
        _ArgStringsValueA = a;
        _ArgStringsValueB = b;
    }
    private static string _ArgStringsValueA = "";
    private static string _ArgStringsValueB = "";
    [Test]
    public void Argument_Strings()
    {
        var a = "A";
        var b = "B";
        _ArgStringsValueA = _ArgStringsValueB = "";
        
        ArgumentParser.Parse(new []{"--strings", a, b});
        Assert.Multiple(() =>
        {
            Assert.That(_ArgStringsValueA, Is.EqualTo(a));
            Assert.That(_ArgStringsValueB, Is.EqualTo(b));
        });
    }
    
    [Argument("numbers")]
    private static void ArgWithNumbers(int i, float f, double d)
    {
        _ArgNumbersValueI = i;
        _ArgNumbersValueF = f;
        _ArgNumbersValueD = d;
    }
    private static int _ArgNumbersValueI = 0;
    private static float _ArgNumbersValueF = 0;
    private static double _ArgNumbersValueD = 0;
    [Test]
    public void Argument_Numbers()
    {
        const int i = 1;
        const float f = 0.1f;
        const double d = 0.22;

        _ArgNumbersValueI = 0;
        _ArgNumbersValueF = 0;
        _ArgNumbersValueD = 0;
        
        ArgumentParser.Parse(new []{"--numbers", i.ToString(),
            f.ToString(CultureInfo.InvariantCulture), d.ToString(CultureInfo.InvariantCulture)});
        Assert.Multiple(() =>
        {
            Assert.That(_ArgNumbersValueI, Is.EqualTo(i));
            Assert.That(_ArgNumbersValueF, Is.EqualTo(f));
            Assert.That(_ArgNumbersValueD, Is.EqualTo(d));
        });
    }

    [Argument("context-only", typeof(string))]
    private static void ContextOnly(string context)
    {
        _ContextOnlyValue = context;
    }

    private static string _ContextOnlyValue = "";

    [Test]
    public void Argument_ContextOnly()
    {
        var context = "context test";
        ArgumentParser.Parse<string>(new []{"--context-only"}, context);
        Assert.That(_ContextOnlyValue, Is.EqualTo(context));
    }

    [Argument("context-with-params", typeof(string))]
    private static void ContextWithParams(string context, int a, int b)
    {
        _ArgContextParamsA = a;
        _ArgContextParamsB = b;
        _ArgContextParamsString = context;
    }

    private static string _ArgContextParamsString = "";
    private static int _ArgContextParamsA = 0;
    private static int _ArgContextParamsB = 0;

    [Test]
    public void Arguments_ContextWithParams()
    {
        int a = 5;
        int b = 10;
        string context = "some context";
        ArgumentParser.Parse<string>(new []{"--context-with-params", a.ToString(), b.ToString()}, context);
        Assert.Multiple(() =>
        {
            Assert.That(_ArgContextParamsString, Is.EqualTo(context));
            Assert.That(_ArgContextParamsA, Is.EqualTo(a));
            Assert.That(_ArgContextParamsB, Is.EqualTo(b));
        });
    }
}