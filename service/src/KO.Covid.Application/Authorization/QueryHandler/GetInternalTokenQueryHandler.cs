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

    public class GetInternalTokenQueryHandler
        : IRequestHandler<GetInternalTokenQuery, string>
    {
        private const string TokenCacheKey = "InternalTokens";

        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;
        private readonly ITelemetryLogger<GetInternalTokenQueryHandler> logger = null;

        private readonly Random randomGenerator = null;

        public GetInternalTokenQueryHandler(
            ICache<Dictionary<string, DateTime>> tokenCache,
            ITelemetryLogger<GetInternalTokenQueryHandler> logger)
        {
            this.tokenCache = tokenCache;
            this.logger = logger;

            this.randomGenerator = new Random();
        }

        public async Task<string> Handle(
            GetInternalTokenQuery request,
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
                var warningMessage = "Could not find any valid internal tokens.";
                this.logger.LogWarning(warningMessage);
                throw new AuthorizationException(warningMessage);
            }

            this.logger.LogInformation(
                "Found {activeInternalTokenCount} active internal tokens.",
                validTokens.Count);

            var index = this.randomGenerator.Next(0, validTokens.Count - 1);
            return validTokens[index];
        }
    }
}
