namespace KO.Covid.Application
{
    using System.Net;

    public static class ApplicationExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        }
    }
}
