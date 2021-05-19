namespace KO.Covid.Application.Exceptions
{
    using System;

    public class AppointmentException : Exception
    {
        public AppointmentException(string message) : base(message)
        {
        }
    }
}
