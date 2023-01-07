using System;

namespace FlakyTest.XUnit.Exceptions
{
    /// <summary>
    /// Exception thrown when a flaky explanation isn't provided.
    /// </summary>
    public class FlakyExplanationException : Exception
    {
        private const string ErrorMessage =
            @"Flaky tests should be the exception, not the 'norm'.  Provide an explanation / ticket number for follow up, etc. so context isn't completely lost.";

        public FlakyExplanationException()
            : base(ErrorMessage) { }
    }
}
