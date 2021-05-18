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

    public class GetAppointmentsCalendarByPincodeQueryHandler
        : IRequestHandler<GetAppointmentsCalendarByPincodeQuery, AppointmentCalendarResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/public/calendarByPin";
        private const string InternalApiAddress = "api/v2/appointment/sessions/calendarByPin";

        private const string TokenCacheKey = "InternalPincodeToken";

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<string> tokenCache = null;

        private readonly ICache<AppointmentCalendarResponse> appointmentsCache = null;

        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        public GetAppointmentsCalendarByPincodeQueryHandler(
            ICache<Credential> credentialCache,
            ICache<string> tokenCache,
            ICache<AppointmentCalendarResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.credentialCache = credentialCache;
            this.tokenCache = tokenCache;

            this.appointmentsCache = appointmentsCache;

            this.appointmentClient = appointmentClient;
            this.baseAddress = baseAddress;
        }

        public async Task<AppointmentCalendarResponse> Handle(
            GetAppointmentsCalendarByPincodeQuery request,
            CancellationToken cancellationToken)
        {
            var appointmentCacheKey = $"Calendar|Pincode|{request.Pincode}|{request.Date}";

            var appointments = await this.appointmentsCache.GetAsync(
                appointmentCacheKey,
                result => result.FromJson<AppointmentCalendarResponse>());

            if (appointments != default)
            {
                return appointments;
            }

            var credential = await this.GetCredentialAsync(request);

            appointments = await this.GetAppointmentsAsync(
                request.Pincode,
                request.Date,
                credential);

            if (appointments.Centers.IsNullOrEmpty())
            {
                return appointments;
            }

            await this.appointmentsCache.SetAsync(
                appointmentCacheKey,
                TimeSpan.FromSeconds(30),
                () => appointments.ToJson());

            return appointments;
        }

        private async Task<Credential> GetCredentialAsync(
            GetAppointmentsCalendarByPincodeQuery request)
        {
            var internalToken = await this.tokenCache.GetAsync(
                TokenCacheKey,
                result => result);

            if (!string.IsNullOrWhiteSpace(internalToken))
            {
                return new Credential { InternalToken = internalToken };
            }

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

        private async Task<AppointmentCalendarResponse> GetAppointmentsAsync(
            string pincode,
            string date,
            Credential credential)
        {
            var apiAddress = ApiAddress;
            var bearerToken = credential.Token;

            if (!string.IsNullOrWhiteSpace(credential.InternalToken))
            {
                apiAddress = InternalApiAddress;
                bearerToken = credential.InternalToken;
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder(
                    $"{this.baseAddress}/{apiAddress}?pincode={pincode}&date={date}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", bearerToken);

            var response = await appointmentClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new GeoException(
                    $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<AppointmentCalendarResponse>();
        }
    }
}
