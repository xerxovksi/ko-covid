namespace KO.Covid.Application.Exceptions
{
    using System;

    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException()
        {
        }

        public TooManyRequestsException(string message) : base(message)
        {
        }

        public TooManyRequestsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
