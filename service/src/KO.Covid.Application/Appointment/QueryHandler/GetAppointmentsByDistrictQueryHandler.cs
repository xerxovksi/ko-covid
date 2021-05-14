namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetAppointmentsByDistrictQueryHandler
        : IRequestHandler<GetAppointmentsByDistrictQuery, AppointmentResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/public/findByDistrict";
        private const string StatesCacheKey = "States";

        private readonly IMediator mediator = null;

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<StatesResponse> statesCache = null;
        private readonly ICache<DistrictsResponse> districtsCache = null;
        private readonly ICache<AppointmentResponse> appointmentsCache = null;

        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        public GetAppointmentsByDistrictQueryHandler(
            IMediator mediator,
            ICache<Credential> credentialCache,
            ICache<StatesResponse> statesCache,
            ICache<DistrictsResponse> districtsCache,
            ICache<AppointmentResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.mediator = mediator;

            this.credentialCache = credentialCache;
            this.statesCache = statesCache;
            this.districtsCache = districtsCache;
            this.appointmentsCache = appointmentsCache;

            this.appointmentClient = appointmentClient;
            this.baseAddress = baseAddress;
        }

        public async Task<AppointmentResponse> Handle(
            GetAppointmentsByDistrictQuery request,
            CancellationToken cancellationToken)
        {
            var appointmentCacheKey = $"{request.StateName}|{request.DistrictName}|{request.Date}";

            var appointments = await this.appointmentsCache.GetAsync(
                appointmentCacheKey,
                result => result.FromJson<AppointmentResponse>());

            if (appointments != default)
            {
                return appointments;
            }

            var state = await this.GetStateAsync(request);
            var district = await this.GetDistrictAsync(request, state);
            var credential = await this.GetCredentialAsync(request);

            appointments = await this.GetAppointmentsAsync(
                district,
                request.Date,
                credential);

            if (appointments.Sessions.IsNullOrEmpty())
            {
                return appointments;
            }

            await this.appointmentsCache.SetAsync(
                appointmentCacheKey,
                TimeSpan.FromMinutes(15),
                () => appointments.ToJson());

            return appointments;
        }

        private async Task<Credential> GetCredentialAsync(GetAppointmentsByDistrictQuery request)
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

        private async Task<State> GetStateAsync(GetAppointmentsByDistrictQuery request)
        {
            var states = await this.statesCache.GetAsync(
                StatesCacheKey,
                result => result.FromJson<StatesResponse>());

            if (states == default)
            {
                states = await this.mediator.Send(
                    new GetStatesQuery { Mobile = request.Mobile });
            }

            var state = states.States
                .Where(item => item.Name.EqualsIgnoreCase(request.StateName))
                .FirstOrDefault();

            if (state == default)
            {
                throw new ArgumentException(
                    $"Invalid {nameof(request.StateName)}: {request.StateName}.");
            }

            return state;
        }

        private async Task<District> GetDistrictAsync(
            GetAppointmentsByDistrictQuery request,
            State state)
        {
            var districts = await this.districtsCache.GetAsync(
                request.StateName,
                result => result.FromJson<DistrictsResponse>());

            if (districts == default)
            {
                districts = await this.mediator.Send(
                    new GetDistrictsQuery
                    {
                        StateName = state.Name,
                        Mobile = request.Mobile
                    });
            }

            var district = districts.Districts
                .Where(item => item.Name.EqualsIgnoreCase(request.DistrictName))
                .FirstOrDefault();

            if (district == default)
            {
                throw new ArgumentException(
                    $"Invalid {nameof(request.DistrictName)}: {request.DistrictName}.");
            }

            return district;
        }

        private async Task<AppointmentResponse> GetAppointmentsAsync(
            District district,
            string date,
            Credential credential)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder(
                    $"{this.baseAddress}/{ApiAddress}?district_id={district.Id}&date={date}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", credential.Token);

            var response = await appointmentClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new GeoException(
                    $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<AppointmentResponse>();
        }
    }
}
