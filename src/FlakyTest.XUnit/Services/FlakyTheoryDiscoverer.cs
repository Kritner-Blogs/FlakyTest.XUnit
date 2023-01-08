using System.Collections.Generic;
using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

/// <summary>
/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> for handling <see cref="FlakyTheoryAttribute"/> decorated
/// test cases.
/// </summary>
public class FlakyTheoryDiscoverer : TheoryDiscoverer
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="diagnosticMessageSink">The default XUnit message sink</param>
    public FlakyTheoryDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink) { }

    /// <inheritdoc />
    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, 
        object[] dataRow)
    {
        int retriesBeforeFail = theoryAttribute.GetNamedArgument<int>(nameof(IFlakyAttribute.RetriesBeforeFail));
        
        return new[]
        {
            new FlakyTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), 
                discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, retriesBeforeFail, dataRow)
        };
    }
}
