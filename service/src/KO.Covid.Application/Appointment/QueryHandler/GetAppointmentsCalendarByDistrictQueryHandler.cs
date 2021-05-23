namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class GetAppointmentsCalendarByDistrictQueryHandler
        : IRequestHandler<GetAppointmentsCalendarByDistrictQuery, AppointmentCalendarResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/calendarByDistrict";

        private readonly IMediator mediator = null;
        private readonly ICache<AppointmentCalendarResponse> appointmentsCache = null;

        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        private readonly ExecutionPolicy policy = null;

        public GetAppointmentsCalendarByDistrictQueryHandler(
            IMediator mediator,
            ICache<AppointmentCalendarResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.mediator = mediator;
            this.appointmentsCache = appointmentsCache;

            this.appointmentClient = appointmentClient;
            this.baseAddress = baseAddress;

            policy = new ExecutionPolicy();
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

            var state = await this.GetStateAsync(request.StateName);
            var district = await this.GetDistrictAsync(state.Name, request.DistrictName);

            appointments = await this.policy.WithRetryAsync(
                operation: async () =>
                {
                    var token = string.IsNullOrWhiteSpace(request.InternalToken)
                    ? await this.mediator.Send(new GetInternalTokenQuery())
                    : request.InternalToken;

                    return await this.GetAppointmentsAsync(
                        district,
                        request.Date,
                        token);
                },
                isSuccessful: _ => true,
                maximumRetryCount: 3,
                shouldThrowIfMaximumRetriesExceeded: true);

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

        private async Task<State> GetStateAsync(string stateName)
        {
            var states = await this.mediator.Send(new GetStatesQuery());
            if (states == default || states.States.IsNullOrEmpty())
            {
                throw new GeoException("Failed to fetch States.");
            }

            var state = states.States
                .Where(item => item.Name.EqualsIgnoreCase(stateName))
                .FirstOrDefault();

            if (state == default)
            {
                throw new ArgumentException($"Invalid State: {stateName}.");
            }

            return state;
        }

        private async Task<District> GetDistrictAsync(string stateName, string districtName)
        {
            var districts = await this.mediator.Send(
                new GetDistrictsQuery { StateName = stateName });
            if (districts == default || districts.Districts.IsNullOrEmpty())
            {
                throw new GeoException("Failed to fetch Districts.");
            }

            var district = districts.Districts
                .Where(item => item.Name.EqualsIgnoreCase(districtName))
                .FirstOrDefault();

            if (district == default)
            {
                throw new ArgumentException(
                    $"Invalid District: {districtName} for State: {stateName}.");
            }

            return district;
        }

        private async Task<AppointmentCalendarResponse> GetAppointmentsAsync(
            District district,
            string date,
            string token)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder(
                    $"{this.baseAddress}/{ApiAddress}?district_id={district.Id}&date={date}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", token);

            var response = await appointmentClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                var errorMessage = $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.";
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized)
                    || response.StatusCode.Equals(HttpStatusCode.Forbidden))
                {
                    throw new AuthorizationException(errorMessage);
                }

                throw new AppointmentException(errorMessage);
            }

            return responseContent.FromJson<AppointmentCalendarResponse>();
        }
    }
}
