using FlakyTest.XUnit.Enums;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Interfaces;

/// <summary>
/// Implements <see cref="IXunitTestCase"/> and adds the additional properties necessary for retrying.
/// </summary>
public interface IMaybeFixedTestCase : IXunitTestCase
{
    /// <summary>
    /// The number of successful runs of the test to "pass" and consider no longer flaky.
    /// </summary>
    int RetriesBeforeDeemingNoLongerFlaky { get; }

    /// <summary>
    /// The status of the test.
    /// </summary>
    FlakyDisposition FlakyDisposition { get; }
}
