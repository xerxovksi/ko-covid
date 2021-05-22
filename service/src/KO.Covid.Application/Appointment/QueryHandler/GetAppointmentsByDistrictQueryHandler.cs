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
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class GetAppointmentsByDistrictQueryHandler
        : IRequestHandler<GetAppointmentsByDistrictQuery, AppointmentResponse>
    {
        private const string ApiAddress = "api/v2/appointment/sessions/public/findByDistrict";

        private readonly IMediator mediator = null;
        private readonly ICache<AppointmentResponse> appointmentsCache = null;

        private readonly HttpClient appointmentClient = null;
        private readonly string baseAddress = null;

        public GetAppointmentsByDistrictQueryHandler(
            IMediator mediator,
            ICache<AppointmentResponse> appointmentsCache,
            HttpClient appointmentClient,
            string baseAddress)
        {
            this.mediator = mediator;
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

            var state = await this.GetStateAsync(request.StateName);
            var district = await this.GetDistrictAsync(state.Name, request.DistrictName);

            var token = string.IsNullOrWhiteSpace(request.PublicToken)
                ? await this.mediator.Send(new GetPublicTokenQuery())
                : request.PublicToken;

            appointments = await this.GetAppointmentsAsync(
                district,
                request.Date,
                token);

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

        private async Task<AppointmentResponse> GetAppointmentsAsync(
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
                throw new AppointmentException(
                    $"Failed to fetch Appointments. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<AppointmentResponse>();
        }
    }
}
