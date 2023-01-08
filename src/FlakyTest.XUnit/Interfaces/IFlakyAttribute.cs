namespace FlakyTest.XUnit.Interfaces;

/// <summary>
/// The required data for marking a test as flaky.
/// </summary>
public interface IFlakyAttribute
{
    /// <summary>
    /// The default number of retries before failing a test case.
    /// </summary>
    public const int DefaultRetriesBeforeFail = 5;
    
    /// <summary>
    /// The explanation as to why the test case is being annotated as flaky.
    /// </summary>
    public string FlakyExplanation { get; }

    /// <summary>
    /// The number of attempts to retry a test case before deeming it a failed test. 
    /// </summary>
    public int RetriesBeforeFail { get; }
}
