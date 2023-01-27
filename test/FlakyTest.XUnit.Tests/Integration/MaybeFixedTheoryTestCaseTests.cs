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

    private static int _counterWhenUsingMaybeFixedTheoryAsyncShouldRunUntilHittingDefaultMax;
    [MaybeFixedTheory]
    [InlineData(true)]
    public async Task WhenUsingMaybeFixedTheoryAsync_ShouldRunUntilHittingDefaultMax(bool value)
    {
        await Task.Delay(1);

        // This is effectively "state" for each "iteration" of the test run, up to the maximum tries.
        _counterWhenUsingMaybeFixedTheoryAsyncShouldRunUntilHittingDefaultMax++;

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

    [MaybeFixedTheory]
    [InlineData(true)]
    public void WhenUsingFlakyTheorySync_ShouldWorkWithExpectedExceptions(bool value)
    {
        var action = ExpectedTestException.ThrowException;

        value.Should().BeTrue();
        action.Should().ThrowExactly<ExpectedTestException>();
    }

    [MaybeFixedTheory]
    [InlineData(true)]
    public async Task WhenUsingFlakyTheoryAsync_ShouldWorkWithExpectedExceptions(bool value)
    {
        var action = ExpectedTestException.ThrowException;

        await Task.Delay(1);

        value.Should().BeTrue();
        action.Should().ThrowExactly<ExpectedTestException>();
    }

    [MaybeFixedTheory(Skip = "skipping")]
    [InlineData(true, Skip = "skipping")]
    public async Task WhenUsedWithSkipTheory_ShouldSkip(bool value)
    {
        await Task.Delay(10_000);
        value.Should().BeFalse("this assert will always fail, but the test should be skipped so it doesn't matter");
    }

    [MaybeFixedTheory]
    [InlineData(true, Skip = "skipping")]
    public async Task WhenUsedWithSkipInlineData_ShouldSkip(bool value)
    {
        await Task.Delay(10_000);
        value.Should().BeFalse("this assert will always fail, but the test should be skipped so it doesn't matter");
    }
}
