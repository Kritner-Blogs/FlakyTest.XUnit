using System;
using System.Threading;
using System.Threading.Tasks;
using FlakyTest.XUnit.Enums;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Models;

/// <summary>
/// Additional properties / implementation against the base <see cref="XunitTestCase"/> to accomodate rerunning.
/// </summary>
[Serializable]
public class MaybeFixedTestCase : XunitTestCase, IMaybeFixedTestCase
{
    /// <summary>
    /// This constructor should not be used.
    /// </summary>
    [Obsolete("Should only ever be implicitly called, never explicitly", true)]
    public MaybeFixedTestCase() { }

    /// <summary>
    /// Constructor
    /// </summary>
    public MaybeFixedTestCase(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        int retriesBeforeDeemingNoLongerFlaky,
        object[]? testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod,
            testMethodArguments)
    {
        RetriesBeforeDeemingNoLongerFlaky = retriesBeforeDeemingNoLongerFlaky;
    }

    /// <inheritdoc />
    public int RetriesBeforeDeemingNoLongerFlaky { get; private set; }

    /// <inheritdoc />
    public FlakyDisposition FlakyDisposition { get; private set; }

    /// <inheritdoc />
    public override async Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink, 
        IMessageBus messageBus,
        object[] constructorArguments, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource)
    {
        return await RunAsync(this, diagnosticMessageSink, messageBus, cancellationTokenSource,
            funcRun: RunFunc(constructorArguments, aggregator, cancellationTokenSource));
    }

    /// <inheritdoc />
    public override void Serialize(IXunitSerializationInfo data)
    {
        base.Serialize(data);

        data.AddValue(nameof(RetriesBeforeDeemingNoLongerFlaky), RetriesBeforeDeemingNoLongerFlaky);
    }

    /// <inheritdoc />
    public override void Deserialize(IXunitSerializationInfo data)
    {
        base.Deserialize(data);

        RetriesBeforeDeemingNoLongerFlaky = data.GetValue<int>(nameof(RetriesBeforeDeemingNoLongerFlaky));
    }
    
    /// <summary>
    /// Initialize runner and run
    /// </summary>
    protected virtual Func<IMessageBus, Task<RunSummary>> RunFunc(
        object[] constructorArguments,
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource)
    {
        return bus => new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments,
                TestMethodArguments, bus, aggregator, cancellationTokenSource)
            .RunAsync();
    }

    private async Task<RunSummary> RunAsync(
        IMaybeFixedTestCase testCase,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        CancellationTokenSource cancellationTokenSource,
        Func<IMessageBus, Task<RunSummary>> funcRun)
    {
        FlakyDisposition = FlakyDisposition.Running;
        var attempt = 0;
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            using var flakyTestMessageBus = new FlakyMessageBus(messageBus);
            attempt++;
            diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                "Running test '{0}'.  Attempt {1} of {2}",
                testCase.DisplayName, attempt, testCase.RetriesBeforeDeemingNoLongerFlaky));

            RunSummary summary = await funcRun(flakyTestMessageBus);

            if (summary.Failed > 0 || attempt >= testCase.RetriesBeforeDeemingNoLongerFlaky - 1)
            {
                if (summary.Failed > 0)
                {
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        "The test '{0}' reports failure after {1} attempts.",
                        testCase.DisplayName, attempt));
                }

                FlakyDisposition = summary.Failed > 0 ? FlakyDisposition.Fail : FlakyDisposition.Success;
                flakyTestMessageBus.Flush();
                return summary;
            }

            // We had a passing test, but have not completed our loop up to the maximum, keep going but log the pass
            diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                "Test '{0}' succeeded on attempt {1}.  Will retry {2} more times to help assure test is not flaky",
                testCase.DisplayName, attempt, testCase.RetriesBeforeDeemingNoLongerFlaky - attempt));
        }

        // Task was cancelled.
        FlakyDisposition = FlakyDisposition.Cancelled;
        diagnosticMessageSink.OnMessage(new DiagnosticMessage(
            "The test '{0}' run attempt was cancelled.",
            testCase.DisplayName));

        return new RunSummary()
        {
            Skipped = 1,
            Total = 1,
        };
    }
}
