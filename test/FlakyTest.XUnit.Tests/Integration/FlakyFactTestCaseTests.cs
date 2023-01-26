using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Tests checking against behaviors/interactions between <see cref="FlakyFactAttribute"/>, <see cref="FlakyFactDiscoverer"/>
/// and <see cref="FlakyTestCase"/>.
/// </summary>
public class FlakyFactTestCaseTests
{
    private static readonly Mock<IBoolReturner> BoolReturner = new();

    static FlakyFactTestCaseTests()
    {
        BoolReturner.SetupSequence(s => s.Get())
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ThrowsAsync(new ExpectedTestException())
            .ReturnsAsync(true);
    }

    [Fact]
    public void WhenUsingFactsSync_ShouldBehaveNormally()
    {
        true.Should().BeTrue();
    }

    [Fact]
    public async Task WhenUsingFactsAsync_ShouldBehaveNormally()
    {
        await Task.Delay(1);
        true.Should().BeTrue();
    }

    [FlakyFact("This test isn't actually flaky, want to make sure happy path first run still works as expected for sync tests")]
    public void WhenUsingFlakyFactSync_ShouldBehaveNormallyOnSuccessfulRun()
    {
        true.Should().BeTrue();
    }

    [FlakyFact("This test isn't actually flaky, want to make sure happy path first run still works as expected for async tests")]
    public async Task WhenUsingFlakyFactAsync_ShouldBehaveNormallyOnSuccessfulRun()
    {
        await Task.Delay(1);
        true.Should().BeTrue();
    }

    private static int _counterWhenUsingFlakyFactShouldFailUntilHittingDefaultMax;
    [FlakyFact("Should fail the number of times until hitting the default max")]
    public void WhenUsingFlakyFact_ShouldFailUntilHittingDefaultMax()
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingFlakyFactShouldFailUntilHittingDefaultMax++;

        // this will "fail" until we hit the max retries, at which point the assertion will be successful.
        _counterWhenUsingFlakyFactShouldFailUntilHittingDefaultMax
            .Should()
            .Be(IFlakyAttribute.DefaultRetriesBeforeFail);
    }

    private const int CustomRetries = 7;
    private static int _counterWhenUsingFlakyFactShouldFailSpecifiedNumberOfTimesBeforeReportingFailure;
    [FlakyFact("Should fail the number of times specified, at which point a success condition is found", CustomRetries)]
    public void WhenUsingFlakyFact_ShouldFailSpecifiedNumberOfTimesBeforeReportingFailure()
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingFlakyFactShouldFailSpecifiedNumberOfTimesBeforeReportingFailure++;

        // Check assumptions
        CustomRetries
            .Should()
            .NotBe(IFlakyAttribute.DefaultRetriesBeforeFail,
                "we're making sure it differs from the default to ensure it's failing the 'correct' number of times");

        // this will "fail" until we hit the max retries, at which point the assertion will be successful.
        _counterWhenUsingFlakyFactShouldFailSpecifiedNumberOfTimesBeforeReportingFailure
            .Should()
            .Be(CustomRetries);
    }
    
    private const int CustomRetriesAsync = 7;
    private static int _counterWhenUsingFlakyFactAsyncShouldFailSpecifiedNumberOfTimesBeforeReportingFailure;
    [FlakyFact("Should fail the number of times specified, at which point a success condition is found", CustomRetries)]
    public void WhenUsingFlakyFactAsync_ShouldFailSpecifiedNumberOfTimesBeforeReportingFailure()
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingFlakyFactAsyncShouldFailSpecifiedNumberOfTimesBeforeReportingFailure++;

        // Check assumptions
        CustomRetries
            .Should()
            .NotBe(IFlakyAttribute.DefaultRetriesBeforeFail,
                "we're making sure it differs from the default to ensure it's failing the 'correct' number of times");

        // this will "fail" until we hit the max retries, at which point the assertion will be successful.
        _counterWhenUsingFlakyFactAsyncShouldFailSpecifiedNumberOfTimesBeforeReportingFailure
            .Should()
            .Be(CustomRetries);
    }

    private static int _counterWhenUsingFlakyFactShouldShortCircuitPass;
    [FlakyFact("Should short circuit pass successfully", 100)]
    public async Task WhenUsingFlakyFact_ShouldShortCircuitPass()
    {
        _counterWhenUsingFlakyFactShouldShortCircuitPass++;

        // The first two returns will be false, the next is true.
        // Assert in such a way that only the final one will be "successful".
        var result = await BoolReturner.Object.Get();
        result.Should().BeTrue($"should only be true on the 4th call. On call {_counterWhenUsingFlakyFactShouldShortCircuitPass}");

        _counterWhenUsingFlakyFactShouldShortCircuitPass
            .Should().Be(4, "this is the number of retries that occurred prior to hitting a success");
    }

    [FlakyFact("Making sure it works with tests that have expected exceptions")]
    public void WhenUsingFlakyFactSync_ShouldWorkWithExpectedExceptions()
    {
        var action = ExpectedTestException.ThrowException;

        action.Should().ThrowExactly<ExpectedTestException>();
    }
    
    [FlakyFact("Making sure it works with tests that have expected exceptions")]
    public async Task WhenUsingFlakyFactAsync_ShouldWorkWithExpectedExceptions()
    {
        var action = ExpectedTestException.ThrowException;

        await Task.Delay(1);
        
        action.Should().ThrowExactly<ExpectedTestException>();
    }

    [FlakyFact("Should work (skip) with skip", Skip = "skipping")]
    public async Task WhenUsedWithSkip_ShouldSkip()
    {
        await Task.Delay(10_000);
        true.Should().BeFalse("this assert will always fail, but the test should be skipped so it doesn't matter");
    }
}
