namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class GetAppointmentsByPincodeQueryHandler
        : IRequestHandler<GetAppointmentsByPincodeQuery, AppointmentResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/public/findByPin";

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<AppointmentResponse> appointmentsCache = null;
        
        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        public GetAppointmentsByPincodeQueryHandler(
            ICache<Credential> credentialCache,
            ICache<AppointmentResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.credentialCache = credentialCache;
            this.appointmentsCache = appointmentsCache;

            this.appointmentClient = appointmentClient;
            this.baseAddress = baseAddress;
        }

        public async Task<AppointmentResponse> Handle(
            GetAppointmentsByPincodeQuery request,
            CancellationToken cancellationToken)
        {
            var appointmentCacheKey = $"Pincode|{request.Pincode}|{request.Date}";

            var appointments = await this.appointmentsCache.GetAsync(
                appointmentCacheKey,
                result => result.FromJson<AppointmentResponse>());

            if (appointments != default)
            {
                return appointments;
            }

            var credential = await this.GetCredentialAsync(request);

            appointments = await this.GetAppointmentsAsync(
                request.Pincode,
                request.Date,
                credential);

            if (appointments.Sessions.IsNullOrEmpty())
            {
                return appointments;
            }

            await this.appointmentsCache.SetAsync(
                appointmentCacheKey,
                AppointmentsCacheDuration,
                () => appointments.ToJson());

            return appointments;
        }

        private async Task<Credential> GetCredentialAsync(GetAppointmentsByPincodeQuery request)
        {
            var credential = await this.credentialCache.GetAsync(
                request.Mobile,
                result => result.FromJson<Credential>());

            if (credential == default || string.IsNullOrEmpty(credential.Token))
            {
                throw new AuthorizationException(
                    request.Mobile,
                    "OTP is either invalid or has expired. Please re-generate.");
            }

            return credential;
        }

        private async Task<AppointmentResponse> GetAppointmentsAsync(
            string pincode,
            string date,
            Credential credential)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder(
                    $"{this.baseAddress}/{ApiAddress}?pincode={pincode}&date={date}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", credential.Token);

            var response = await appointmentClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new AppointmentException(
                    $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<AppointmentResponse>();
        }
    }
}
