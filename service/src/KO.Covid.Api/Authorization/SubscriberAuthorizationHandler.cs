namespace KO.Covid.Api.Authorization
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Domain;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    internal struct AuthorizationHeaders
    {
        internal string Mobile { get; set; }

        internal string Otp { get; set; }
    }

    public class SubscriberAuthorizationHandler
        : AuthorizationHandler<SubscriberAuthorizationRequirement>, IAuthorizationHandler
    {
        private const string MobileKey = "mobile";
        private const string OtpKey = "otp";

        private readonly IMediator mediator = null;
        private readonly IHttpContextAccessor contextAccessor = null;
        private readonly ITelemetryLogger<SubscriberAuthorizationHandler> logger = null;

        public SubscriberAuthorizationHandler(
            IMediator mediator,
            IHttpContextAccessor contextAccessor,
            ITelemetryLogger<SubscriberAuthorizationHandler> logger)
        {
            this.mediator = mediator;
            this.contextAccessor = contextAccessor;
            this.logger = logger;
        }

        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubscriberAuthorizationRequirement requirement)
        {
            try
            {
                var result = this.Validate(contextAccessor.HttpContext.Request);

                var credential = await this.mediator.Send(
                    new GetCredentialQuery { Mobile = result.Mobile });

                if (credential == null
                    || string.IsNullOrWhiteSpace(credential.Otp)
                    || string.IsNullOrWhiteSpace(credential.Token)
                    || !credential.Otp.Equals(result.Otp))
                {
                    this.logger.LogWarning(
                        $"Credential is either invalid or has expired for subscriber's mobile: {result.Mobile}.");

                    context.Fail();
                }

                context.Succeed(requirement);
            }
            catch (Exception exception)
            {
                this.logger.LogWarning(exception, exception.Message);
                context.Fail();
            }
        }

        private AuthorizationHeaders Validate(HttpRequest request)
        {
            var mobileValue = request.Headers.GetValue(MobileKey);
            if (string.IsNullOrWhiteSpace(mobileValue))
            {
                this.logger.LogInformation(
                    $"Authorization value for {nameof(MobileKey)} is missing.");

                throw new AuthorizationException(
                    "One or more of the authorization values are missing.");
            }

            var otpValue = request.Headers.GetValue(OtpKey);
            if (string.IsNullOrWhiteSpace(otpValue))
            {
                this.logger.LogInformation(
                    $"Authorization value for {nameof(OtpKey)} is missing.");

                throw new AuthorizationException(
                    "One or more of the authorization values are missing.");
            }

            return new AuthorizationHeaders
            {
                Mobile = mobileValue,
                Otp = otpValue
            };
        }
    }
}
