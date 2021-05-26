namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using MediatR;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class ConfirmOtpCommandHandler
        : IRequestHandler<ConfirmOtpCommand, bool>
    {
        private const string ApiAddress = "api/v2/auth/public/confirmOTP";

        private readonly ICache<Credential> credentialCache = null;
        private readonly IMediator mediator = null;

        private readonly HttpClient otpClient = null;
        private readonly string baseAddress = null;

        public ConfirmOtpCommandHandler(
            ICache<Credential> credentialCache,
            IMediator mediator,
            HttpClient otpClient,
            string baseAddress)
        {
            this.credentialCache = credentialCache;
            this.mediator = mediator;

            this.otpClient = otpClient;
            this.baseAddress = baseAddress;
        }

        public async Task<bool> Handle(
            ConfirmOtpCommand request,
            CancellationToken cancellationToken)
        {
            var credential = await this.credentialCache.GetAsync(
                request.Mobile,
                result => result.FromJson<Credential>());

            if (credential == default)
            {
                throw new AuthorizationException(
                    $"OTP is either invalid or has expired for mobile: {request.Mobile}. Please re-generate.");
            }

            credential.Token = await this.GetTokenAsync(request, credential.TransactionId);
            await this.credentialCache.SetAsync(
                request.Mobile,
                CredentialCacheDuration,
                () => credential.ToJson());

            await this.mediator.Send(
                new AddPublicTokenCommand { PublicToken = credential.Token });

            return await this.mediator.Send(
                new AddActiveUserCommand { Mobile = request.Mobile });
        }

        private async Task<string> GetTokenAsync(
            ConfirmOtpCommand request,
            string transactionId)
        {
            var payload = new ConfirmOtpRequest
            {
                Otp = request.Otp.ToSHA256(),
                TransactionId = transactionId
            };

            var response = await otpClient.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new UriBuilder($"{this.baseAddress}/{ApiAddress}").Uri,
                    Content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json")
                });

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == false)
            {
                throw new AuthorizationException(
                    $"OTP is either invalid or has expired for mobile: {request.Mobile}. Please re-generate. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            var otpResponse = responseContent.FromJson<ConfirmOtpResponse>();
            if (otpResponse == null || string.IsNullOrWhiteSpace(otpResponse.Token))
            {
                throw new AuthorizationException(
                    $"OTP is either invalid or has expired for mobile: {request.Mobile}. Please re-generate. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return otpResponse.Token;
        }
    }
}
