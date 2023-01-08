using Xunit.Sdk;

namespace FlakyTest.XUnit.Interfaces;

public interface IFlakyTestCase : IXunitTestCase
{
    /// <summary>
    /// The maximum number of retries before deeming a test case as a failed test.
    /// </summary>
    int RetriesBeforeFail { get; }
}
