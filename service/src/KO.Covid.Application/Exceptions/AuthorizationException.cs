namespace KO.Covid.Application.Exceptions
{
    using System;

    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
