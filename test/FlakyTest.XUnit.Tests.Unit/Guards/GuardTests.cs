using FlakyTest.XUnit.Exceptions;
using FlakyTest.XUnit.Guards;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Unit.Guards;

/// <summary>
/// Tests against the <see cref="Guard"/> methods.
/// </summary>
public class GuardTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("JIRA-42 should address flakyness")]
    public void AgainstNotProvidedFlakyExplanation_WhenGivenValidExplanation_ShouldNotThrow(string explanation)
    {
        var action = () => Guard.AgainstNotProvidedFlakyExplanation(explanation);

        action.Should().NotThrow("this is a valid explanation provided");
    }

    [Fact]
    public void AgainstNotProvidedFlakyExplanation_WhenGivenZeroLengthExplanation_ShouldThrow()
    {
        var action = () => Guard.AgainstNotProvidedFlakyExplanation(string.Empty);

        action.Should().ThrowExactly<FlakyExplanationException>();
    }

    [Fact]
    public void AgainstNotProvidedFlakyExplanation_WhenGivenNullExplanation_ShouldThrow()
    {
        var action = () => Guard.AgainstNotProvidedFlakyExplanation(null!);

        action.Should().ThrowExactly<FlakyExplanationException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(42)]
    public void AgainstInvalidRetries_WhenGivenValidNumber_ShouldNotThrow(int retriesBeforeFail)
    {
        var action = () => Guard.AgainstInvalidRetries(retriesBeforeFail);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-7)]
    [InlineData(-42)]
    public void AgainstInvalidRetries_WhenGivenNegativeNumber_ShouldThrow(int retriesBeforeFail)
    {
        var action = () => Guard.AgainstInvalidRetries(retriesBeforeFail);

        action.Should().ThrowExactly<InvalidNumberOfRetriesException>();
    }

    [Fact]
    public void AgainstInvalidRetries_WhenGivenZero_ShouldThrow()
    {
        var action = () => Guard.AgainstInvalidRetries(0);

        action.Should().ThrowExactly<InvalidNumberOfRetriesException>();
    }
}
