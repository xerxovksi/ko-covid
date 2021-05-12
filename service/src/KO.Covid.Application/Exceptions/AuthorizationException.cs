namespace KO.Covid.Application.Exceptions
{
    using System;

    public class AuthorizationException : Exception
    {
        public AuthorizationException(string mobile, string reason)
            : base($"Failed to authorize mobile: {mobile}. Reason: {reason}")
        {
        }
    }
}
