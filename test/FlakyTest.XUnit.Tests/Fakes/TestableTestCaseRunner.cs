using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Tests.Fakes;

/// <summary>
/// An implementation of <see cref="TestCaseRunner{TTestCase}"/> to support expected disposition tests.
/// </summary>
public class TestableTestCaseRunner<TErrorTestCase> : TestCaseRunner<TErrorTestCase>
    where TErrorTestCase : XunitTestCase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestableTestCaseRunner"/> class.
    /// </summary>
    /// <param name="testCase">The test case that the lambda represents.</param>
    /// <param name="messageSink">The message sink.</param>
    /// <param name="messageBus">The message bus to report run status to.</param>
    /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
    /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
    public TestableTestCaseRunner(TErrorTestCase testCase,
        IMessageSink messageSink,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        : base(testCase, messageBus, aggregator, cancellationTokenSource)
    {
        MessageSink = messageSink;
    }

    private IMessageSink MessageSink { get; }

    protected override async Task<RunSummary> RunTestAsync()
    {
        return await TestCase.RunAsync(MessageSink, MessageBus, null, Aggregator, CancellationTokenSource);
    }
}
