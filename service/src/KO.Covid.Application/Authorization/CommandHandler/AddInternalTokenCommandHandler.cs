namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class AddInternalTokenCommandHandler
        : IRequestHandler<AddInternalTokenCommand, bool>
    {
        private const string ValidationState = "West Bengal";
        private const string ValidationDistrict = "Kolkata";

        private readonly IMediator mediator = null;
        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;

        public AddInternalTokenCommandHandler(
            IMediator mediator,
            ICache<Dictionary<string, DateTime>> tokenCache)
        {
            this.mediator = mediator;
            this.tokenCache = tokenCache;
        }

        public async Task<bool> Handle(
            AddInternalTokenCommand request,
            CancellationToken cancellationToken)
        {
            await this.ValidateAsync(request.InternalToken);

            var timeToLive = DateTime.Now.Add(TokenCacheDuration);

            var tokens = await this.tokenCache.GetAsync(
                InternalTokensCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            if (tokens.IsNullOrEmpty())
            {
                return await this.tokenCache.SetAsync(
                    InternalTokensCacheKey,
                    TokenCacheDuration,
                    () => new Dictionary<string, DateTime>
                    {
                        { request.InternalToken, timeToLive }
                    }.ToJson());
            }

            tokens[request.InternalToken] = timeToLive;
            
            return await this.tokenCache.SetAsync(
                InternalTokensCacheKey,
                TokenCacheDuration,
                () => tokens.ToJson());
        }

        private async Task ValidateAsync(string internalToken) =>
            await this.mediator.Send(
                new GetAppointmentsCalendarByDistrictQuery
                {
                    StateName = ValidationState,
                    DistrictName = ValidationDistrict,
                    Date = DateTime.Now.ToIndianDate(),
                    InternalToken = internalToken
                });
    }
}
