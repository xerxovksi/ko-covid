namespace KO.Covid.Application.Geo
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
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

    public class GetDistrictsQueryHandler
        : IRequestHandler<GetDistrictsQuery, DistrictsResponse>
    {
        private const string ApiAddress = "api/v2/admin/location/districts";

        private readonly IMediator mediator = null;
        private readonly ICache<DistrictsResponse> districtsCache = null;

        private readonly HttpClient geoClient = null;
        private readonly string baseAddress = null;

        public GetDistrictsQueryHandler(
            IMediator mediator,
            ICache<DistrictsResponse> districtsCache,
            HttpClient geoClient,
            string baseAddress)
        {
            this.mediator = mediator;
            this.districtsCache = districtsCache;

            this.geoClient = geoClient;
            this.baseAddress = baseAddress;
        }

        public async Task<DistrictsResponse> Handle(
            GetDistrictsQuery request,
            CancellationToken cancellationToken)
        {
            var districts = await this.districtsCache.GetAsync(
                request.StateName,
                result => result.FromJson<DistrictsResponse>());

            if (districts != default)
            {
                return districts;
            }

            var state = await this.GetStateAsync(request);

            var token = string.IsNullOrWhiteSpace(request.PublicToken)
                ? await this.mediator.Send(new GetPublicTokenQuery())
                : request.PublicToken;

            districts = await this.GetDistrictsAsync(state, token);

            await this.districtsCache.SetAsync(
                request.StateName,
                GeoCacheDuration,
                () => districts.ToJson());

            return districts;
        }

        private async Task<State> GetStateAsync(GetDistrictsQuery request)
        {
            var states = await this.mediator.Send(new GetStatesQuery());
            if (states == default || states.States.IsNullOrEmpty())
            {
                throw new GeoException("Failed to fetch States.");
            }

            var state = states.States
                .Where(item => item.Name.EqualsIgnoreCase(request.StateName))
                .FirstOrDefault();

            if (state == default)
            {
                throw new ArgumentException($"Invalid {nameof(request.StateName)}: {request.StateName}.");
            }

            return state;
        }

        private async Task<DistrictsResponse> GetDistrictsAsync(State state, string token)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{this.baseAddress}/{ApiAddress}/{state.Id}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", token);

            var response = await geoClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new GeoException(
                    $"Failed to fetch Districts. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<DistrictsResponse>();
        }
    }
}
