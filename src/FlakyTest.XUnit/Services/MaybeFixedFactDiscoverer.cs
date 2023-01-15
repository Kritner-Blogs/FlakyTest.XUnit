using System.Collections.Generic;
using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

/// <summary>
/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> for handling <see cref="MaybeFixedFactAttribute"/> decorated
/// test cases.
/// </summary>
/// <inheritdoc />
public class MaybeFixedFactDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _messageSink;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageSink">The default XUnit message sink</param>
    public MaybeFixedFactDiscoverer(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    /// <inheritdoc />
    public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        var retriesBeforeDeemingNoLongerFlaky = factAttribute.GetNamedArgument<int>(nameof(IMaybeFixedAttribute.RetriesBeforeDeemingNoLongerFlaky));

        IXunitTestCase testCase = new MaybeFixedTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, retriesBeforeDeemingNoLongerFlaky);

        return new[] { testCase };
    }
}
