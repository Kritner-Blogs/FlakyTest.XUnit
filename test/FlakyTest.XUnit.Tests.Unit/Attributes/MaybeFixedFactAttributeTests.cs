using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Unit.Attributes;

/// <summary>
/// Tests against the <see cref="MaybeFixedFactAttribute"/> class.
/// </summary>
public class MaybeFixedFactAttributeTests
{
    private MaybeFixedFactAttribute? _sut;

    [Fact]
    public void ctor_WhenGivenNoRetryCount_ShouldUseDefault()
    {
        _sut = new MaybeFixedFactAttribute();

        _sut.RetriesBeforeDeemingNoLongerFlaky.Should().Be(IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(42)]
    public void ctor_WhenGivenRetryCount_ShouldSetRetryCount(int retryCount)
    {
        _sut = new MaybeFixedFactAttribute(retryCount);

        _sut.RetriesBeforeDeemingNoLongerFlaky.Should().Be(retryCount);
    }
}
