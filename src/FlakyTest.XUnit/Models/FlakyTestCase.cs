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
public class FlakyTestCase : XunitTestCase, IFlakyTestCase
{
    /// <summary>
    /// Message template string for running a test attempt
    /// </summary>
    public const string MessageTemplateRunningTestAttemptOf = "Running test '{0}'.  Attempt {1} of {2}";

    /// <summary>
    /// Message template string for a failed test case 
    /// </summary>
    public const string MessageTemplateTestReportsFailureAfterAttempts = "The test '{0}' reports failure after {1} attempts.";

    /// <summary>
    /// This constructor should not be used.
    /// </summary>
    [Obsolete("Should only ever be implicitly called, never explicitly", true)]
    public FlakyTestCase() { }

    /// <summary>
    /// Constructor
    /// </summary>
    public FlakyTestCase(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        int retriesBeforeFail,
        object[]? testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod,
            testMethodArguments)
    {
        RetriesBeforeFail = retriesBeforeFail;
    }

    /// <inheritdoc />
    public int RetriesBeforeFail { get; private set; }

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

        data.AddValue(nameof(RetriesBeforeFail), RetriesBeforeFail);
    }

    /// <inheritdoc />
    public override void Deserialize(IXunitSerializationInfo data)
    {
        base.Deserialize(data);

        RetriesBeforeFail = data.GetValue<int>(nameof(RetriesBeforeFail));
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
        IFlakyTestCase testCase,
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
                MessageTemplateRunningTestAttemptOf,
                testCase.DisplayName, attempt, testCase.RetriesBeforeFail));

            RunSummary summary = await funcRun(flakyTestMessageBus);

            // If we have a passing test, or we've attempted (and failed) the maximum retries, return a result.
            if (summary.Failed == 0 || attempt >= testCase.RetriesBeforeFail)
            {
                if (summary.Failed > 0)
                {
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        MessageTemplateTestReportsFailureAfterAttempts,
                        testCase.DisplayName, attempt));
                }

                FlakyDisposition = summary.Failed > 0 ? FlakyDisposition.Fail : FlakyDisposition.Success;
                flakyTestMessageBus.Flush();
                return summary;
            }

            // We had a failing test, log that.  If this point is hit there are additional attempts to make on the test,
            // prior ot deeming it a failed test.
            diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                "Test '{0}' failed on attempt {1}.  Will retry {2} more times for a passing test",
                testCase.DisplayName, attempt, testCase.RetriesBeforeFail - attempt));
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
