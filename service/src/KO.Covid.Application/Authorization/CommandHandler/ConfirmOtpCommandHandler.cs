namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using MediatR;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ConfirmOtpCommandHandler
        : IRequestHandler<ConfirmOtpCommand, bool>
    {
        private const string ApiAddress = "api/v2/auth/public/confirmOTP";
        private const string ActiveCacheKey = "ActiveUsers";

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<HashSet<string>> activeCache = null;
        private readonly HttpClient otpClient = null;
        private readonly string baseAddress = null;

        public ConfirmOtpCommandHandler(
            ICache<Credential> credentialCache,
            ICache<HashSet<string>> activeCache,
            HttpClient otpClient,
            string baseAddress)
        {
            this.credentialCache = credentialCache;
            this.activeCache = activeCache;
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
                    request.Mobile,
                    "OTP is either invalid or has expired. Please re-generate.");
            }

            credential = await this.GetCredentialAsync(request, credential.TransactionId);
            await this.credentialCache.SetAsync(
                request.Mobile,
                TimeSpan.FromDays(3),
                () => credential.ToJson());

            await this.SetActiveCacheAsync(request.Mobile);

            return true;
        }

        private async Task<Credential> GetCredentialAsync(
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
                    request.Mobile,
                    $"Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            var otpResponse = responseContent.FromJson<ConfirmOtpResponse>();

            return new Credential
            {
                Mobile = request.Mobile,
                TransactionId = transactionId,
                Otp = payload.Otp,
                Token = otpResponse.Token
            };
        }

        private async Task SetActiveCacheAsync(string mobile)
        {
            var activeMobiles = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<HashSet<string>>());

            if (activeMobiles == default)
            {
                await this.activeCache.SetAsync(
                    ActiveCacheKey,
                    TimeSpan.FromHours(12),
                    () => new List<string> { mobile }.ToJson());

                return;
            }

            if (activeMobiles.Contains(mobile))
            {
                return;
            }

            await this.activeCache.SetAsync(
                ActiveCacheKey,
                TimeSpan.FromHours(12),
                () => activeMobiles.ToJson());
        }
    }
}
