using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Unit.Attributes;

/// <summary>
/// Tests against the <see cref="MaybeFixedTheoryAttribute"/> class.
/// </summary>
public class MaybeFixedTheoryAttributeTests
{
    private MaybeFixedTheoryAttribute? _sut;

    [Fact]
    public void ctor_WhenGivenNoRetryCount_ShouldUseDefault()
    {
        _sut = new MaybeFixedTheoryAttribute();

        _sut.RetriesBeforeDeemingNoLongerFlaky.Should().Be(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(42)]
    public void ctor_WhenGivenRetryCount_ShouldSetRetryCount(int retryCount)
    {
        _sut = new MaybeFixedTheoryAttribute(retryCount);

        _sut.RetriesBeforeDeemingNoLongerFlaky.Should().Be(retryCount);
    }
}
