namespace KO.Covid.Application.Otp
{
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Models;
    using MediatR;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class GenerateOtpCommandHandler
        : IRequestHandler<GenerateOtpCommand, GenerateOtpResponse>
    {
        private const string ApiAddress = "api/v2/auth/public/generateOTP";

        private readonly HttpClient otpClient = null;
        private readonly string baseAddress = null;

        public GenerateOtpCommandHandler(HttpClient otpClient, string baseAddress)
        {
            this.otpClient = otpClient;
            this.baseAddress = baseAddress;
        }

        public async Task<GenerateOtpResponse> Handle(
            GenerateOtpCommand request,
            CancellationToken cancellationToken)
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

            return JsonConvert.DeserializeObject<GenerateOtpResponse>(responseContent);
        }
    }
}
