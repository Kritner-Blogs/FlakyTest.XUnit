using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Tests.Fakes;

/// <summary>
/// Fake MaybeFixed test case that always reports success
/// </summary>
public class SuccessMaybeFixedTestCase : MaybeFixedTestCase
{
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public SuccessMaybeFixedTestCase()
    {
    }

    public SuccessMaybeFixedTestCase(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        int retriesBeforeFail,
        object[]? testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, retriesBeforeFail, testMethodArguments)
    {
    }

    protected override Func<IMessageBus, Task<RunSummary>> RunFunc(object[] constructorArguments, ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        return bus => Task.FromResult(new RunSummary() { Total = 1 });
    }
}
