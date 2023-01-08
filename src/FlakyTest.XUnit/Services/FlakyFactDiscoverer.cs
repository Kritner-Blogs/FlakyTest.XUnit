using System.Collections.Generic;
using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

public class FlakyFactDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _messageSink;

    public FlakyFactDiscoverer(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        int retriesBeforeFail = factAttribute.GetNamedArgument<int>(nameof(FlakyFactAttribute.RetriesBeforeFail));

        IXunitTestCase testCase = new FlakyTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, retriesBeforeFail);

        return new[] { testCase };
    }
}
