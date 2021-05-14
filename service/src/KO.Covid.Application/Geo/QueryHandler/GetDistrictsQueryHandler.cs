namespace KO.Covid.Application.Geo
{
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

    public class GetDistrictsQueryHandler
        : IRequestHandler<GetDistrictsQuery, DistrictsResponse>
    {
        private const string ApiAddress = "api/v2/admin/location/districts";
        private const string StatesCacheKey = "States";

        private readonly IMediator mediator = null;

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<StatesResponse> statesCache = null;
        private readonly ICache<DistrictsResponse> districtsCache = null;

        private readonly HttpClient geoClient = null;
        private readonly string baseAddress = null;

        public GetDistrictsQueryHandler(
            IMediator mediator,
            ICache<Credential> credentialCache,
            ICache<StatesResponse> statesCache,
            ICache<DistrictsResponse> districtsCache,
            HttpClient geoClient,
            string baseAddress)
        {
            this.mediator = mediator;

            this.credentialCache = credentialCache;
            this.statesCache = statesCache;
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
            var credential = await this.GetCredentialAsync(request);

            districts = await this.GetDistrictsAsync(state, credential);
            
            await this.districtsCache.SetAsync(
                request.StateName,
                TimeSpan.FromDays(1),
                () => districts.ToJson());

            return districts;
        }

        private async Task<Credential> GetCredentialAsync(GetDistrictsQuery request)
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

        private async Task<State> GetStateAsync(GetDistrictsQuery request)
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
                throw new ArgumentException($"Invalid {nameof(request.StateName)}: {request.StateName}.");
            }

            return state;
        }

        private async Task<DistrictsResponse> GetDistrictsAsync(State state, Credential credential)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{this.baseAddress}/{ApiAddress}/{state.Id}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", credential.Token);

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
