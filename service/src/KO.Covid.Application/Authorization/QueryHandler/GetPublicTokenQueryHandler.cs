namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetPublicTokenQueryHandler
        : IRequestHandler<GetPublicTokenQuery, string>
    {
        private const string TokenCacheKey = "PublicTokens";

        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;
        private readonly ITelemetryLogger<GetPublicTokenQueryHandler> logger = null;

        private readonly Random randomGenerator = null;

        public GetPublicTokenQueryHandler(
            ICache<Dictionary<string, DateTime>> tokenCache,
            ITelemetryLogger<GetPublicTokenQueryHandler> logger)
        {
            this.tokenCache = tokenCache;
            this.logger = logger;

            this.randomGenerator = new Random();
        }

        public async Task<string> Handle(
            GetPublicTokenQuery request,
            CancellationToken cancellationToken)
        {
            var tokens = await this.tokenCache.GetAsync(
                TokenCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            var validTokens = tokens
                .Where(item => DateTime.Now <= item.Value)
                .Select(item => item.Key)
                .ToList();

            if (validTokens.IsNullOrEmpty())
            {
                var warningMessage = "Could not find any valid public tokens.";
                this.logger.LogWarning(warningMessage);
                throw new AuthorizationException(warningMessage);
            }

            this.logger.LogInformation(
                "Found {activePublicTokenCount} active public tokens.",
                validTokens.Count);

            var index = this.randomGenerator.Next(0, validTokens.Count - 1);
            return validTokens[index];
        }
    }
}
