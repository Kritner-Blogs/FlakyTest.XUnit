using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Tests checking against behaviors/interactions between <see cref="MaybeFixedFactAttribute"/>, <see cref="MaybeFixedFactDiscoverer"/>
/// and <see cref="MaybeFixedTestCase"/>.
/// </summary>
public class MaybeFixedFactTestCaseTests
{
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

    [MaybeFixedFact]
    public void WhenUsingMaybeFixedFactSync_ShouldBehaveNormallyOnSuccessfulRun()
    {
        true.Should().BeTrue();
    }

    [MaybeFixedFact]
    public async Task WhenUsingMaybeFixedFactAsync_ShouldBehaveNormallyOnSuccessfulRun()
    {
        await Task.Delay(1);
        true.Should().BeTrue();
    }

    private static int _counterWhenUsingMaybeFixedFactShouldRunUntilHittingDefaultMax;
    [MaybeFixedFact]
    public void WhenUsingMaybeFixedFact_ShouldRunUntilHittingDefaultMax()
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingMaybeFixedFactShouldRunUntilHittingDefaultMax++;

        _counterWhenUsingMaybeFixedFactShouldRunUntilHittingDefaultMax
            .Should()
            .BeLessOrEqualTo(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky);
    }

    private const int CustomRetries = 7;
    private static int _counterWhenUsingMaybeFixedFactShouldPassExpectedNumberOfTimes;
    [MaybeFixedFact(CustomRetries)]
    public void WhenUsingMaybeFixedFact_ShouldPassExpectedNumberOfTimes()
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingMaybeFixedFactShouldPassExpectedNumberOfTimes++;

        // Check assumptions
        CustomRetries
            .Should()
            .NotBe(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky,
                "we're making sure it differs from the default");

        _counterWhenUsingMaybeFixedFactShouldPassExpectedNumberOfTimes
            .Should()
            .BeLessOrEqualTo(CustomRetries);
    }
    
    [MaybeFixedFact(Skip = "skipping")]
    public async Task WhenUsedWithSkip_ShouldSkip()
    {
        await Task.Delay(10_000);
        true.Should().BeFalse("this assert will always fail, but the test should be skipped so it doesn't matter");
    }
}
