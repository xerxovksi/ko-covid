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

    using static KO.Covid.Application.Constants;

    public class GetPublicTokenQueryHandler
        : IRequestHandler<GetPublicTokenQuery, string>
    {
        private readonly ITokenLoadBalancer loadBalancer = null;
        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;
        private readonly ITelemetryLogger<GetPublicTokenQueryHandler> logger = null;

        public GetPublicTokenQueryHandler(
            ITokenLoadBalancer loadBalancer,
            ICache<Dictionary<string, DateTime>> tokenCache,
            ITelemetryLogger<GetPublicTokenQueryHandler> logger)
        {
            this.loadBalancer = loadBalancer;
            this.tokenCache = tokenCache;
            this.logger = logger;
        }

        public async Task<string> Handle(
            GetPublicTokenQuery request,
            CancellationToken cancellationToken)
        {
            var tokens = await this.tokenCache.GetAsync(
                PublicTokensCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            var validTokens = tokens
                ?.Where(item => DateTime.Now <= item.Value)
                ?.Select(item => item.Key)
                ?.ToList();

            if (validTokens.IsNullOrEmpty())
            {
                var warningMessage = "Could not find any valid public tokens.";
                this.logger.LogWarning(warningMessage);
                throw new AuthorizationException(warningMessage);
            }

            var index = this.loadBalancer.GetIndex(validTokens.Count);
            this.logger.LogInformation(
                "Found {activePublicTokenCount} active public tokens. Selected index: {activePublicTokenIndex}.",
                validTokens.Count,
                index);

            return validTokens[index];
        }
    }
}
