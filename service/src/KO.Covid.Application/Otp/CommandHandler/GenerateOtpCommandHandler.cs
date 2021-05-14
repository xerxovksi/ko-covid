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

    public class GenerateOtpCommandHandler
        : IRequestHandler<GenerateOtpCommand, bool>
    {
        private const string ApiAddress = "api/v2/auth/public/generateOTP";

        private readonly ICache<Credential> cache = null;
        private readonly HttpClient otpClient = null;
        private readonly string baseAddress = null;

        public GenerateOtpCommandHandler(
            ICache<Credential> cache,
            HttpClient otpClient,
            string baseAddress)
        {
            this.cache = cache;
            this.otpClient = otpClient;
            this.baseAddress = baseAddress;
        }

        public async Task<bool> Handle(
            GenerateOtpCommand request,
            CancellationToken cancellationToken)
        {
            var credential = await this.GetCredentialAsync(request);
            await this.cache.SetAsync(
                request.Mobile,
                TimeSpan.FromMinutes(50),
                () => credential.ToJson());

            return true;
        }

        private async Task<Credential> GetCredentialAsync(GenerateOtpCommand request)
        {
            var payload = new GenerateOtpRequest { Mobile = request.Mobile };
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

            var otpResponse = responseContent.FromJson<GenerateOtpResponse>();
            
            return new Credential
            {
                Mobile = request.Mobile,
                TransactionId = otpResponse.TransactionId
            };
        }
    }
}
