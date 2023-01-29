using FlakyTest.XUnit.Enums;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Interfaces;

/// <summary>
/// Implements <see cref="IXunitTestCase"/> and adds the additional properties necessary for retrying.
/// </summary>
public interface IFlakyTestCase : IXunitTestCase
{
    /// <summary>
    /// The maximum number of retries before deeming a test case as a failed test.
    /// </summary>
    int RetriesBeforeFail { get; }

    /// <summary>
    /// The status of the test.
    /// </summary>
    FlakyDisposition FlakyDisposition { get; }
}
