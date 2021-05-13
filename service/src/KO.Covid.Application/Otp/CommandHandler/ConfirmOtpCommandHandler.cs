namespace KO.Covid.Application.Otp
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

    public class ConfirmOtpCommandHandler
        : IRequestHandler<ConfirmOtpCommand, bool>
    {
        private const string ApiAddress = "api/v2/auth/public/confirmOTP";

        private readonly ICache<Credential> cache = null;
        private readonly HttpClient otpClient = null;
        private readonly string baseAddress = null;

        public ConfirmOtpCommandHandler(
            ICache<Credential> cache,
            HttpClient otpClient,
            string baseAddress)
        {
            this.cache = cache;
            this.otpClient = otpClient;
            this.baseAddress = baseAddress;
        }

        public async Task<bool> Handle(
            ConfirmOtpCommand request,
            CancellationToken cancellationToken)
        {
            var credential = await this.cache.GetAsync(
                request.Mobile,
                result => result.FromJson<Credential>());

            if (credential == default)
            {
                throw new AuthorizationException(
                    request.Mobile,
                    "OTP is either invalid or has expired. Please re-generate.");
            }

            var payload = new ConfirmOtpRequest
            {
                Otp = request.Otp.ToSHA256(),
                TransactionId = credential.TransactionId
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

            var result = responseContent.FromJson<ConfirmOtpResponse>();
            await this.cache.SetAsync(
                request.Mobile,
                TimeSpan.FromMinutes(50),
                () => new Credential
                {
                    Mobile = request.Mobile,
                    TransactionId = credential.TransactionId,
                    Otp = payload.Otp,
                    Token = result.Token
                }.ToJson());

            return true;
        }
    }
}
