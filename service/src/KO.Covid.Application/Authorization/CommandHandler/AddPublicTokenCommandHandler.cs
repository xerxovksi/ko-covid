namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class AddPublicTokenCommandHandler
        : IRequestHandler<AddPublicTokenCommand, bool>
    {
        private const string TokenCacheKey = "PublicTokens";

        private readonly IMediator mediator = null;
        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;

        public AddPublicTokenCommandHandler(
            IMediator mediator,
            ICache<Dictionary<string, DateTime>> tokenCache)
        {
            this.mediator = mediator;
            this.tokenCache = tokenCache;
        }

        public async Task<bool> Handle(
            AddPublicTokenCommand request,
            CancellationToken cancellationToken)
        {
            await this.ValidateAsync(request.PublicToken);

            var timeToLive = DateTime.Now.Add(ActiveCacheDuration);

            var tokens = await this.tokenCache.GetAsync(
                TokenCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            if (tokens.IsNullOrEmpty())
            {
                return await this.tokenCache.SetAsync(
                    TokenCacheKey,
                    TokenCacheDuration,
                    () => new Dictionary<string, DateTime>
                    {
                        { request.PublicToken, timeToLive }
                    }.ToJson());
            }

            tokens[request.PublicToken] = timeToLive;

            return await this.tokenCache.SetAsync(
                TokenCacheKey,
                TokenCacheDuration,
                () => tokens.ToJson());
        }

        private async Task ValidateAsync(string publicToken) =>
            await this.mediator.Send(
                new GetStatesQuery { PublicToken = publicToken });
    }
}
