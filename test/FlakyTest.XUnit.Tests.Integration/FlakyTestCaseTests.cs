using FlakyTest.XUnit.Attributes;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Integration;

public class FlakyTestCaseTests
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
}
