using System.Collections.Generic;
using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

/// <summary>
/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> for handling <see cref="MaybeFixedTheoryAttribute"/> decorated
/// test cases.
/// </summary>
/// <inheritdoc />
public class MaybeFixedTheoryDiscoverer : TheoryDiscoverer
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="diagnosticMessageSink">The default XUnit message sink</param>
    public MaybeFixedTheoryDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink) { }

    /// <inheritdoc />
    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute,
        object[] dataRow)
    {
        int retriesBeforeDeemingNoLongerFlaky = theoryAttribute.GetNamedArgument<int>(nameof(IMaybeFixedAttribute.RetriesBeforeDeemingNoLongerFlaky));

        return new[]
        {
            new MaybeFixedTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, retriesBeforeDeemingNoLongerFlaky, dataRow)
        };
    }
}
