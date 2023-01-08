using System;

namespace FlakyTest.XUnit.Exceptions;

/// <summary>
/// Exception thrown when a the number of retries provided is less than or equal to zero.
/// </summary>
public class InvalidNumberOfRetriesException : Exception
{
    private const string ErrorMessage = "Number of retries should not be less or equal to zero.";

    /// <summary>
    /// Constructor
    /// </summary>
    public InvalidNumberOfRetriesException() : base(ErrorMessage) { }
}
