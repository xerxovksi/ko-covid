namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class RemoveInactiveTokensCommandHandler
        : IRequestHandler<RemoveInactiveTokensCommand, bool>
    {
        private const string PublicTokensCacheKey = "PublicTokens";
        private const string InternalTokensCacheKey = "InternalTokens";

        private readonly ICache<Dictionary<string, DateTime>> tokenCache = null;

        public RemoveInactiveTokensCommandHandler(ICache<Dictionary<string, DateTime>> tokenCache) =>
            this.tokenCache = tokenCache;

        public async Task<bool> Handle(
            RemoveInactiveTokensCommand request,
            CancellationToken cancellationToken)
        {
            var currentTime = DateTime.Now;
            
            return await this.RemoveInactiveTokens(PublicTokensCacheKey, currentTime)
                && await this.RemoveInactiveTokens(InternalTokensCacheKey, currentTime);
        }

        private async Task<bool> RemoveInactiveTokens(
            string cacheKey,
            DateTime currentTime)
        {
            var tokens = await this.tokenCache.GetAsync(
                cacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            if (tokens.IsNullOrEmpty())
            {
                return true;
            }

            var inactiveTokens = tokens
                .Where(item => currentTime > item.Value)
                .Select(item => item.Key)
                .ToList();

            var isSuccessful = true;
            foreach (var token in inactiveTokens)
            {
                isSuccessful &= await this.tokenCache.RemoveAsync(token);
            }

            return isSuccessful;
        }
    }
}
