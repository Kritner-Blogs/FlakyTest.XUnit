using FlakyTest.XUnit.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Tests.Fakes;

/// <summary>
/// Fake flaky test case that returns a fail response after an amount of time.
/// </summary>
public class DelayedFlakyTestCase : FlakyTestCase
{
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public DelayedFlakyTestCase()
    {
    }

    public DelayedFlakyTestCase(
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
        return async bus =>
        {
            await Task.Delay(10_000);
            return new RunSummary() { Total = 1, Failed = 1 };
        };
    }
}
