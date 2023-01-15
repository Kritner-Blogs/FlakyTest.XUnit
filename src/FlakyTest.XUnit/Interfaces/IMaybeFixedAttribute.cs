namespace FlakyTest.XUnit.Interfaces;

/// <summary>
/// <para>
/// The attributes that implement this interface can be used to run a test up to a specified number of times
/// to attempt to determine if the test is *still* flaky (or fixed).
/// </para>
/// <para>
/// A situation where you *had* a flaky test, have made corrections to the underlying code or test, and want to confirm
/// that the test is no longer flaky, would be a good use case for such attributes.
/// </para>
/// </summary>
public interface IMaybeFixedAttribute
{
    /// <summary>
    /// The default number of successful runs of the test to "pass" and consider no longer flaky.
    /// </summary>
    public const int DefaultRetriesBeforeDeemingNoLongerFlaky = 10;

    /// <summary>
    /// The number of successful runs of the test to "pass" and consider no longer flaky. 
    /// </summary>
    public int RetriesBeforeDeemingNoLongerFlaky { get; }
}
