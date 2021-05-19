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

    using static KO.Covid.Application.Constants;

    public class GetAppointmentsCalendarByDistrictQueryHandler
        : IRequestHandler<GetAppointmentsCalendarByDistrictQuery, AppointmentCalendarResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/public/calendarByDistrict";
        private const string InternalApiAddress = "api/v2/appointment/sessions/calendarByDistrict";

        private const string StatesCacheKey = "States";
        private const string TokenCacheKey = "InternalDistrictToken";

        private readonly IMediator mediator = null;

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<string> tokenCache = null;

        private readonly ICache<StatesResponse> statesCache = null;
        private readonly ICache<DistrictsResponse> districtsCache = null;
        private readonly ICache<AppointmentCalendarResponse> appointmentsCache = null;

        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        public GetAppointmentsCalendarByDistrictQueryHandler(
            IMediator mediator,
            ICache<Credential> credentialCache,
            ICache<string> tokenCache,
            ICache<StatesResponse> statesCache,
            ICache<DistrictsResponse> districtsCache,
            ICache<AppointmentCalendarResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.mediator = mediator;

            this.credentialCache = credentialCache;
            this.tokenCache = tokenCache;

            this.statesCache = statesCache;
            this.districtsCache = districtsCache;
            this.appointmentsCache = appointmentsCache;

            this.appointmentClient = appointmentClient;
            this.baseAddress = baseAddress;
        }

        public async Task<AppointmentCalendarResponse> Handle(
            GetAppointmentsCalendarByDistrictQuery request,
            CancellationToken cancellationToken)
        {
            var appointmentCacheKey = $"Calendar|{request.StateName}|{request.DistrictName}|{request.Date}";

            var appointments = await this.appointmentsCache.GetAsync(
                appointmentCacheKey,
                result => result.FromJson<AppointmentCalendarResponse>());

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

            if (appointments.Centers.IsNullOrEmpty())
            {
                return appointments;
            }

            await this.appointmentsCache.SetAsync(
                appointmentCacheKey,
                AppointmentsCacheDuration,
                () => appointments.ToJson());

            return appointments;
        }

        private async Task<Credential> GetCredentialAsync(
            GetAppointmentsCalendarByDistrictQuery request)
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

        private async Task<State> GetStateAsync(
            GetAppointmentsCalendarByDistrictQuery request)
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
            GetAppointmentsCalendarByDistrictQuery request,
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

        private async Task<AppointmentCalendarResponse> GetAppointmentsAsync(
            District district,
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
                    $"{this.baseAddress}/{apiAddress}?district_id={district.Id}&date={date}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", bearerToken);

            var response = await appointmentClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new AppointmentException(
                    $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<AppointmentCalendarResponse>();
        }
    }
}
