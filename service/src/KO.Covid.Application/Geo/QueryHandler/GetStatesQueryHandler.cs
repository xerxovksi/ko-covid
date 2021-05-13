namespace KO.Covid.Application.Geo
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

    public class GetStatesQueryHandler
        : IRequestHandler<GetStatesQuery, StatesResponse>
    {
        private const string ApiAddress = "api/v2/admin/location/states";
        private const string StatesCacheKey = "States";

        private readonly ICache<Credential> credentialCache = null;
        private readonly ICache<StatesResponse> statesCache = null;

        private readonly HttpClient geoClient = null;
        private readonly string baseAddress = null;

        public GetStatesQueryHandler(
            ICache<Credential> credentialCache,
            ICache<StatesResponse> statesCache,
            HttpClient geoClient,
            string baseAddress)
        {
            this.credentialCache = credentialCache;
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

            var credential = await this.credentialCache.GetAsync(
                request.Mobile,
                result => result.FromJson<Credential>());

            if (credential == default || string.IsNullOrEmpty(credential.Token))
            {
                throw new AuthorizationException(
                    request.Mobile,
                    "OTP is either invalid or has expired. Please re-generate.");
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{this.baseAddress}/{ApiAddress}").Uri,
            };
            requestMessage.Headers.TryAddWithoutValidation("Bearer", credential.Token);

            var response = await geoClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new GeoException(
                    $"Failed to fetch States. Status Code: {(int)response.StatusCode}. Content: {responseContent}.");
            }

            var result = responseContent.FromJson<StatesResponse>();
            await this.statesCache.SetAsync(StatesCacheKey, TimeSpan.FromDays(1), () => responseContent);

            return result;
        }
    }
}
