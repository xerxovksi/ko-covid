namespace KO.Covid.Application
{
    using KO.Covid.Domain;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public static class ApplicationExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        }

        public static string ToIndianDate(this DateTime value) =>
            value.ToString("dd-MM-yyyy");

        public static async Task<T> ParseAsync<T>(this Stream stream)
        {
            if (stream == default)
            {
                return default;
            }

            try
            {
                using var reader = new StreamReader(stream);
                var body = await reader.ReadToEndAsync();
                
                return body.FromJson<T>();
            }
            catch
            {
                return default;
            }
        }
    }
}
