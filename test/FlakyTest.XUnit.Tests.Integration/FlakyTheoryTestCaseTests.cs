using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Tests checking against behaviors/interactions between <see cref="FlakyTheoryAttribute"/>, <see cref="FlakyTheoryDiscoverer"/>
/// and <see cref="FlakyTestCase"/>.
/// </summary>
public class FlakyTheoryTestCaseTests
{
    private static readonly Mock<IBoolReturner> BoolReturner = new();

    static FlakyTheoryTestCaseTests()
    {
        BoolReturner.SetupSequence(s => s.Get())
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);
    }

    [Theory]
    [InlineData(true)]
    public void WhenUsingTheorySync_ShouldBehaveNormally(bool value)
    {
        value.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    public async Task WhenUsingTheoryAsync_ShouldBehaveNormally(bool value)
    {
        await Task.Delay(1);
        value.Should().BeTrue();
    }

    [FlakyTheory("This test isn't actually flaky, want to make sure happy path first run still works as expected for sync tests")]
    [InlineData(true)]
    public void WhenUsingFlakyTheorySync_ShouldBehaveNormallyOnSuccessfulRun(bool value)
    {
        value.Should().BeTrue();
    }

    [FlakyTheory("This test isn't actually flaky, want to make sure happy path first run still works as expected for async tests")]
    [InlineData(true)]
    public async Task WhenUsingFlakyTheoryAsync_ShouldBehaveNormallyOnSuccessfulRun(bool value)
    {
        await Task.Delay(1);
        value.Should().BeTrue();
    }

    private static int _counterWhenUsingFlakyTheoryShouldFailUntilHittingDefaultMax;

    [FlakyTheory("Should fail the number of times until hitting the default max")]
    [InlineData(true)]
    public void WhenUsingFlakyTheory_ShouldFailUntilHittingDefaultMax(bool value)
    {
        value.Should().BeTrue();

        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingFlakyTheoryShouldFailUntilHittingDefaultMax++;

        // this will "fail" until we hit the max retries, at which point the assertion will be successful.
        _counterWhenUsingFlakyTheoryShouldFailUntilHittingDefaultMax
            .Should()
            .Be(IFlakyAttribute.DefaultRetriesBeforeFail);
    }

    private const int CustomRetries = 7;
    private static int _counterWhenUsingFlakyTheoryShouldFailSpecifiedNumberOfTimesBeforeReportingFailure;
    [FlakyTheory("Should fail the number of times specified, at which point a success condition is found", CustomRetries)]
    [InlineData(true)]
    public void WhenUsingFlakyTheory_ShouldFailSpecifiedNumberOfTimesBeforeReportingFailure(bool value)
    {
        value.Should().BeTrue();

        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingFlakyTheoryShouldFailSpecifiedNumberOfTimesBeforeReportingFailure++;

        // Check assumptions
        CustomRetries
            .Should()
            .NotBe(IFlakyAttribute.DefaultRetriesBeforeFail,
                "we're making sure it differs from the default to ensure it's failing the 'correct' number of times");

        // this will "fail" until we hit the max retries, at which point the assertion will be successful.
        _counterWhenUsingFlakyTheoryShouldFailSpecifiedNumberOfTimesBeforeReportingFailure
            .Should()
            .Be(CustomRetries);
    }

    private static int _counterWhenUsingFlakyTheoryShouldShortCircuitPass;
    [FlakyTheory("Should short circuit pass successfully", 100)]
    [InlineData(true)]
    public async Task WhenUsingFlakyTheory_ShouldShortCircuitPass(bool value)
    {
        value.Should().BeTrue();

        _counterWhenUsingFlakyTheoryShouldShortCircuitPass++;

        // The first two returns will be false, the next is true.
        // Assert in such a way that only the final one will be "successful".
        var result = await BoolReturner.Object.Get();
        result.Should().BeTrue($"should only be true on the 3rd call. On call {_counterWhenUsingFlakyTheoryShouldShortCircuitPass}");

        _counterWhenUsingFlakyTheoryShouldShortCircuitPass
            .Should().Be(3, "this is the number of retries that occurred prior to hitting a success");
    }
}
