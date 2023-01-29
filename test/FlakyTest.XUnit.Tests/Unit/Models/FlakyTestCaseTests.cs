using FlakyTest.XUnit.Models;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Tests.Unit.Models;

/// <summary>
/// Unit tests against <see cref="FlakyTestCase"/>
/// </summary>
public class FlakyTestCaseTests
{
    private readonly Mock<IXunitSerializationInfo> _xunitSerializationInfo = new();
    private readonly ITestMethod _testMethod = Mocks.TestMethod("MockType", "MockMethod");

    public FlakyTestCaseTests()
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
                    v.AddValue(nameof(FlakyTestCase.RetriesBeforeFail), retriesBeforeFail, null),
                Times.Once);
    }

    [Fact]
    public void ShouldDeserializeWithExpectedCalls()
    {
        var sut = GetSystemUnderTest(5);

        sut.Deserialize(_xunitSerializationInfo.Object);

        _xunitSerializationInfo
            .Verify(v =>
                v.GetValue<int>(nameof(FlakyTestCase.RetriesBeforeFail)), Times.Once);
    }
    
    private FlakyTestCase GetSystemUnderTest(int retriesBeforeFail)
    {
        var sut = new FlakyTestCase(
            new NullMessageSink(),
            TestMethodDisplay.Method,
            TestMethodDisplayOptions.All,
            _testMethod,
            retriesBeforeFail,
            null);

        return sut;
    }
}
