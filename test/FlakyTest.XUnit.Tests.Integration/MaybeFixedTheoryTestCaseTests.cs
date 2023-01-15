using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Tests checking against behaviors/interactions between <see cref="MaybeFixedTheoryAttribute"/>, <see cref="MaybeFixedTheoryDiscoverer"/>
/// and <see cref="MaybeFixedTestCase"/>.
/// </summary>
public class MaybeFixedTheoryTestCaseTests
{
    [Theory]
    [InlineData(true)]
    public void WhenUsingTheorysSync_ShouldBehaveNormally(bool value)
    {
        value.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    public async Task WhenUsingTheorysAsync_ShouldBehaveNormally(bool value)
    {
        await Task.Delay(1);
        value.Should().BeTrue();
    }

    [MaybeFixedTheory]
    [InlineData(true)]
    public void WhenUsingMaybeFixedTheorySync_ShouldBehaveNormallyOnSuccessfulRun(bool value)
    {
        value.Should().BeTrue();
    }

    [MaybeFixedTheory]
    [InlineData(true)]
    public async Task WhenUsingMaybeFixedTheoryAsync_ShouldBehaveNormallyOnSuccessfulRun(bool value)
    {
        await Task.Delay(1);
        value.Should().BeTrue();
    }

    private static int _counterWhenUsingMaybeFixedTheoryShouldRunUntilHittingDefaultMax;
    [MaybeFixedTheory]
    [InlineData(true)]
    public void WhenUsingMaybeFixedTheory_ShouldRunUntilHittingDefaultMax(bool value)
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingMaybeFixedTheoryShouldRunUntilHittingDefaultMax++;

        value.Should().BeTrue();

        _counterWhenUsingMaybeFixedTheoryShouldRunUntilHittingDefaultMax
            .Should()
            .BeLessOrEqualTo(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky);
    }

    private const int CustomRetries = 7;
    private static int _counterWhenUsingMaybeFixedTheoryShouldPassExpectedNumberOfTimes;
    [MaybeFixedTheory(CustomRetries)]
    [InlineData(true)]
    public void WhenUsingMaybeFixedTheory_ShouldPassExpectedNumberOfTimes(bool value)
    {
        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingMaybeFixedTheoryShouldPassExpectedNumberOfTimes++;

        value.Should().BeTrue();

        // Check assumptions
        CustomRetries
            .Should()
            .NotBe(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky,
                "we're making sure it differs from the default");

        _counterWhenUsingMaybeFixedTheoryShouldPassExpectedNumberOfTimes
            .Should()
            .BeLessOrEqualTo(CustomRetries);
    }
}
