using System;
using System.Threading;
using System.Threading.Tasks;
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
    [Obsolete("Should only ever be implicitly called, never explicitly", true)]
    public FlakyTestCase() { }
    
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

    public int RetriesBeforeFail { get; private set; }

    public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
        object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        return await RunAsync(this, diagnosticMessageSink, messageBus, cancellationTokenSource,
            funcRun: bus => new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments,
                    TestMethodArguments, bus, aggregator, cancellationTokenSource)
                .RunAsync());
    }
        

    public override void Serialize(IXunitSerializationInfo data)
    {
        base.Serialize(data);

        data.AddValue(nameof(RetriesBeforeFail), RetriesBeforeFail);
    }

    public override void Deserialize(IXunitSerializationInfo data)
    {
        base.Deserialize(data);

        RetriesBeforeFail = data.GetValue<int>(nameof(RetriesBeforeFail));
    }

    private static async Task<RunSummary> RunAsync(
        IFlakyTestCase testCase,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        CancellationTokenSource cancellationTokenSource,
        Func<IMessageBus, Task<RunSummary>> funcRun)
    {
        var attempt = 0;
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            using var flakyTestMessageBus = new FlakyMessageBus(messageBus);
            attempt++;
            diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                "Running test '{0}'.  Attempt {1} of {2}",
                testCase.DisplayName, attempt, testCase.RetriesBeforeFail));

            RunSummary summary = await funcRun(flakyTestMessageBus);

            // If we have a passing test, or we've attempted (and failed) the maximum retries, return a result.
            if (summary.Failed == 0 || attempt >= testCase.RetriesBeforeFail)
            {
                if (summary.Failed > 0)
                {
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        "The test '{0}' reports failure after {1} attempts.",
                        testCase.DisplayName, testCase.RetriesBeforeFail));
                }

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
        diagnosticMessageSink.OnMessage(new DiagnosticMessage(
            "The test '{0}' run attempt was cancelled.",
            testCase.DisplayName));
        
        return new RunSummary()
        {
            Failed = 1,
        };
    }
}
