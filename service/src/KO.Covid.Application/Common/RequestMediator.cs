namespace KO.Covid.Application
{
    using FluentValidation;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Models;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class RequestMediator : IRequestMediator
    {
        private readonly IMediator mediator = null;
        private readonly ITelemetryLogger<RequestMediator> logger = null;

        public RequestMediator(
            IMediator mediator,
            ITelemetryLogger<RequestMediator> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<IActionResult> SendAsync<T>(
            IRequest<T> request,
            Action validate = null,
            Func<T, string> successLogMessage = null,
            Func<T, object[]> successLogParameters = null)
        {
            try
            {
                validate?.Invoke();

                var result = await this.mediator.Send(request);

                if (successLogMessage != default
                    && !string.IsNullOrWhiteSpace(successLogMessage(result)))
                {
                    this.logger.LogInformation(
                        successLogMessage(result),
                        successLogParameters == default
                            ? null
                            : successLogParameters(result));
                }

                return new OkObjectResult(
                    ApiResponse<T>.GetSuccessResponse(
                        result,
                        HttpStatusCode.OK));
            }
            catch (Exception exception)
            {
                if (exception is AuthorizationException)
                {
                    this.logger.LogWarning(exception, exception.Message);
                    return new ObjectResult(
                        this.GetErrorResponse<T>(
                            HttpStatusCode.Unauthorized,
                            exception.Message))
                    { StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                if (exception is ArgumentException
                    || exception is ValidationException)
                {
                    this.logger.LogWarning(exception, exception.Message);
                    return new BadRequestObjectResult(
                        this.GetErrorResponse<T>(
                            HttpStatusCode.BadRequest,
                            exception.Message));
                }

                this.logger.LogError(exception, exception.Message);
                return new ObjectResult(
                    this.GetErrorResponse<T>(
                        HttpStatusCode.InternalServerError,
                        exception.Message))
                { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        private ApiResponse<T> GetErrorResponse<T>(
            HttpStatusCode httpStatusCode,
            string errorMessage) =>
            ApiResponse<T>.GetErrorResponse(
                httpStatusCode,
                new Error
                {
                    Name = httpStatusCode.ToString(),
                    Description = errorMessage
                });
    }
}
