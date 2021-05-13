namespace KO.Covid.Application.Exceptions
{
    using System;

    public class GeoException : Exception
    {
        public GeoException(string message) : base(message)
        {
        }
    }
}
