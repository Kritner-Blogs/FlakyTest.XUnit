using FlakyTest.XUnit.Enums;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Tests.Fakes;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using DiagnosticMessage = Xunit.Sdk.DiagnosticMessage;
using NullMessageSink = Xunit.Sdk.NullMessageSink;
using TestMethodDisplay = Xunit.Sdk.TestMethodDisplay;
using TestMethodDisplayOptions = Xunit.Sdk.TestMethodDisplayOptions;

namespace FlakyTest.XUnit.Tests.Unit.Models;

/// <summary>
/// Unit tests against <see cref="FlakyTestCase"/>
/// </summary>
public class FlakyTestCaseTests
{
    private readonly Mock<IXunitSerializationInfo> _xunitSerializationInfo = new();
    private readonly ITestMethod _testMethod = Mocks.TestMethod("MockType", "MockMethod");

    public FlakyTestCaseTests()
    {
        _xunitSerializationInfo
            .Setup(s => s.GetValue<string>("DefaultMethodDisplay"))
            .Returns("Method");
        _xunitSerializationInfo
            .Setup(s => s.GetValue<string>("DefaultMethodDisplayOptions"))
            .Returns("None");
        _xunitSerializationInfo
            .Setup(s => s.GetValue<ITestMethod>("TestMethod"))
            .Returns(_testMethod);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(42)]
    public void ShouldSerializeWithExpectedCalls(int retriesBeforeFail)
    {
        var sut = GetSystemUnderTest(retriesBeforeFail);

        sut.Serialize(_xunitSerializationInfo.Object);

        _xunitSerializationInfo
            .Verify(v =>
                    v.AddValue(nameof(FlakyTestCase.RetriesBeforeFail), retriesBeforeFail, null),
                Times.Once);
    }

    [Fact]
    public void ShouldDeserializeWithExpectedCalls()
    {
        var sut = GetSystemUnderTest(7);

        sut.Deserialize(_xunitSerializationInfo.Object);

        _xunitSerializationInfo
            .Verify(v =>
                v.GetValue<int>(nameof(FlakyTestCase.RetriesBeforeFail)), Times.Once);
    }

    [Fact]
    public async Task ShouldRunAndReportFailureToDispositionProperty()
    {
        var messageBus = new Mock<IMessageBus>();
        messageBus
            .Setup(s => s.QueueMessage(It.IsAny<IMessageSinkMessage>()))
            .Returns(true);
        var messageSink = new Mock<IMessageSink>();

        var sut = GetFailTestCase(1, messageSink.Object);
        var runner = new TestableTestCaseRunner<FlakyTestCase>(
            sut,
            messageSink.Object,
            messageBus.Object,
            new ExceptionAggregator(),
            new CancellationTokenSource());


        sut.FlakyDisposition.Should().Be(FlakyDisposition.NotStarted);
        await runner.RunAsync();


        sut.FlakyDisposition.Should().Be(FlakyDisposition.Fail);
    }

    [Fact]
    public async Task ShouldRunAndReportSuccessToDispositionProperty()
    {
        var messageBus = new Mock<IMessageBus>();
        messageBus
            .Setup(s => s.QueueMessage(It.IsAny<IMessageSinkMessage>()))
            .Returns(true);
        var messageSink = new Mock<IMessageSink>();

        var sut = GetSuccessTestCase(1, messageSink.Object);
        var runner = new TestableTestCaseRunner<FlakyTestCase>(
            sut,
            messageSink.Object,
            messageBus.Object,
            new ExceptionAggregator(),
            new CancellationTokenSource());


        sut.FlakyDisposition.Should().Be(FlakyDisposition.NotStarted);
        await runner.RunAsync();


        sut.FlakyDisposition.Should().Be(FlakyDisposition.Success);
    }

    [Fact]
    public async Task ShouldRunAndReportCancelledToDispositionProperty()
    {
        var messageBus = new Mock<IMessageBus>();
        messageBus
            .Setup(s => s.QueueMessage(It.IsAny<IMessageSinkMessage>()))
            .Returns(true);
        var messageSink = new Mock<IMessageSink>();

        var tokenSource = new CancellationTokenSource();
        var sut = GetDelayedTestCase(1, messageSink.Object);
        var runner = new TestableTestCaseRunner<FlakyTestCase>(
            sut,
            messageSink.Object,
            messageBus.Object,
            new ExceptionAggregator(),
            tokenSource);


        sut.FlakyDisposition.Should().Be(FlakyDisposition.NotStarted);
        tokenSource.Cancel();
        await runner.RunAsync();


        sut.FlakyDisposition.Should().Be(FlakyDisposition.Cancelled);
    }

    private FlakyTestCase GetSystemUnderTest(
        int retriesBeforeFail = IFlakyAttribute.DefaultRetriesBeforeFail,
        IMessageSink? messageSink = null)
    {
        return new FlakyTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            retriesBeforeFail,
            null);
    }

    private FailFlakyTestCase GetFailTestCase(
        int retriesBeforeFail = IFlakyAttribute.DefaultRetriesBeforeFail,
        IMessageSink? messageSink = null)
    {
        return new FailFlakyTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            retriesBeforeFail,
            null);
    }

    private SuccessFlakyTestCase GetSuccessTestCase(
        int retriesBeforeFail = IFlakyAttribute.DefaultRetriesBeforeFail,
        IMessageSink? messageSink = null)
    {
        return new SuccessFlakyTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            retriesBeforeFail,
            null);
    }

    private DelayedFlakyTestCase GetDelayedTestCase(
        int retriesBeforeFail = IFlakyAttribute.DefaultRetriesBeforeFail,
        IMessageSink? messageSink = null)
    {
        return new DelayedFlakyTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            retriesBeforeFail,
            null);
    }
}
