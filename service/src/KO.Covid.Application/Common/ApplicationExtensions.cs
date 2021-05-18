namespace KO.Covid.Application
{
    using System;
    using System.Net;

    public static class ApplicationExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        }

        public static string ToIndianDate(this DateTime value) =>
            value.ToString("dd-MM-yyyy");
    }
}
