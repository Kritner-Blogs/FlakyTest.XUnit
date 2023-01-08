using System.Collections.Generic;
using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

/// <summary>
/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> for handling <see cref="FlakyFactAttribute"/> decorated
/// test cases.
/// </summary>
/// <inheritdoc />
public class FlakyFactDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _messageSink;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageSink">The default XUnit message sink</param>
    public FlakyFactDiscoverer(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    /// <inheritdoc />
    public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        var retriesBeforeFail = factAttribute.GetNamedArgument<int>(nameof(FlakyFactAttribute.RetriesBeforeFail));

        IXunitTestCase testCase = new FlakyTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, retriesBeforeFail);

        return new[] { testCase };
    }
}
