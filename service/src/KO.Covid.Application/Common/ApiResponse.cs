namespace KO.Covid.Application
{
    using KO.Covid.Application.Models;
    using System.Collections.Generic;
    using System.Net;

    public class ApiResponse<T>
    {
        public T Results { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public IEnumerable<Error> Errors { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public static ApiResponse<T> GetSuccessResponse(
            T data,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            Dictionary<string, string> headers = default) =>
            new()
            {
                Results = data,
                HttpStatusCode = statusCode,
                Headers = headers
            };

        public static ApiResponse<T> GetErrorResponse(
            HttpStatusCode statusCode,
            Error error,
            Dictionary<string, string> headers = default) =>
            new()
            {
                Errors = new List<Error> { error },
                HttpStatusCode = statusCode,
                Headers = headers
            };

        public static ApiResponse<T> GetErrorResponse(
            HttpStatusCode statusCode,
            IEnumerable<Error> errors,
            Dictionary<string, string> headers = default) =>
            new()
            {
                Errors = errors,
                HttpStatusCode = statusCode,
                Headers = headers
            };
    }
}
