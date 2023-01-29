using FlakyTest.XUnit.Enums;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Tests.Fakes;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using NullMessageSink = Xunit.Sdk.NullMessageSink;
using TestMethodDisplay = Xunit.Sdk.TestMethodDisplay;
using TestMethodDisplayOptions = Xunit.Sdk.TestMethodDisplayOptions;

namespace FlakyTest.XUnit.Tests.Unit.Models;

/// <summary>
/// Unit tests against <see cref="MaybeFixedTestCase"/>
/// </summary>
public class MaybeFixedTestCaseTests
{
    private readonly Mock<IXunitSerializationInfo> _xunitSerializationInfo = new();
    private readonly ITestMethod _testMethod = Mocks.TestMethod("MockType", "MockMethod");

    public MaybeFixedTestCaseTests()
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
                    v.AddValue(nameof(MaybeFixedTestCase.RetriesBeforeDeemingNoLongerFlaky), retriesBeforeFail, null),
                Times.Once);
    }

    [Fact]
    public void ShouldDeserializeWithExpectedCalls()
    {
        var sut = GetSystemUnderTest(7);

        sut.Deserialize(_xunitSerializationInfo.Object);

        _xunitSerializationInfo
            .Verify(v =>
                v.GetValue<int>(nameof(MaybeFixedTestCase.RetriesBeforeDeemingNoLongerFlaky)), Times.Once);
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
        var runner = new TestableTestCaseRunner<MaybeFixedTestCase>(
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
        var runner = new TestableTestCaseRunner<MaybeFixedTestCase>(
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
        var runner = new TestableTestCaseRunner<MaybeFixedTestCase>(
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
    
    private MaybeFixedTestCase GetSystemUnderTest(
        int defaultRetriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky, 
        IMessageSink? messageSink = null)
    {
        return new MaybeFixedTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            defaultRetriesBeforeDeemingNoLongerFlaky,
            null);
    }

    private FailMaybeFixedTestCase GetFailTestCase(
        int defaultRetriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky,
        IMessageSink? messageSink = null)
    {
        return new FailMaybeFixedTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            defaultRetriesBeforeDeemingNoLongerFlaky,
            null);
    }
    
    private SuccessMaybeFixedTestCase GetSuccessTestCase(
        int defaultRetriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky,
        IMessageSink? messageSink = null)
    {
        return new SuccessMaybeFixedTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            defaultRetriesBeforeDeemingNoLongerFlaky,
            null);
    }
    
    private DelayedMaybeFixedTestCase GetDelayedTestCase(
        int defaultRetriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky,
        IMessageSink? messageSink = null)
    {
        return new DelayedMaybeFixedTestCase(
            messageSink ?? new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            defaultRetriesBeforeDeemingNoLongerFlaky,
            null);
    }
}
