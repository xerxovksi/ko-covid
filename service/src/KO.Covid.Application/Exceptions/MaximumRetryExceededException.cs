namespace KO.Covid.Application.Exceptions
{
    using System;

    public class MaximumRetryExceededException : Exception
    {
        public MaximumRetryExceededException(int maximumRetryCount)
            : base($"Maximum retry attempts of {maximumRetryCount} exceeded for operation.")
        {
        }
    }
}
