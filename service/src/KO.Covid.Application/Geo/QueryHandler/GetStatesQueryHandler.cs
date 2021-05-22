namespace KO.Covid.Application.Geo
{
    using KO.Covid.Application.Authorization;
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

    public class GetStatesQueryHandler
        : IRequestHandler<GetStatesQuery, StatesResponse>
    {
        private const string ApiAddress = "api/v2/admin/location/states";
        private const string StatesCacheKey = "States";

        private readonly IMediator mediator = null;
        private readonly ICache<StatesResponse> statesCache = null;

        private readonly HttpClient geoClient = null;
        private readonly string baseAddress = null;

        public GetStatesQueryHandler(
            IMediator mediator,
            ICache<StatesResponse> statesCache,
            HttpClient geoClient,
            string baseAddress)
        {
            this.mediator = mediator;
            this.statesCache = statesCache;

            this.geoClient = geoClient;
            this.baseAddress = baseAddress;
        }

        public async Task<StatesResponse> Handle(
            GetStatesQuery request,
            CancellationToken cancellationToken)
        {
            var states = await this.statesCache.GetAsync(
                StatesCacheKey,
                result => result.FromJson<StatesResponse>());

            if (states != default)
            {
                return states;
            }

            var token = string.IsNullOrWhiteSpace(request.PublicToken)
                ? await this.mediator.Send(new GetPublicTokenQuery())
                : request.PublicToken;
            
            states = await this.GetStatesAsync(token);
            if (states == default || states.States.IsNullOrEmpty())
            {
                return states;
            }
            
            await this.statesCache.SetAsync(
                StatesCacheKey,
                GeoCacheDuration,
                () => states.ToJson());

            return states;
        }

        private async Task<StatesResponse> GetStatesAsync(string token)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{this.baseAddress}/{ApiAddress}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", token);

            var response = await geoClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new GeoException(
                    $"Failed to fetch States. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            return responseContent.FromJson<StatesResponse>();
        }
    }
}
