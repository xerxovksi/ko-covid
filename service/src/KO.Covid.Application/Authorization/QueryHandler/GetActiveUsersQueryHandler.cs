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

    using static KO.Covid.Application.Constants;

    public class GetActiveUsersQueryHandler
        : IRequestHandler<GetActiveUsersQuery, HashSet<string>>
    {
        private readonly ICache<Dictionary<string, DateTime>> activeCache = null;

        public GetActiveUsersQueryHandler(ICache<Dictionary<string, DateTime>> activeCache) =>
            this.activeCache = activeCache;

        public async Task<HashSet<string>> Handle(
            GetActiveUsersQuery request,
            CancellationToken cancellationToken)
        {
            var activeUsers = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            return activeUsers
                ?.Where(item => DateTime.Now <= item.Value)
                ?.Select(item => item.Key)
                ?.ToHashSet();
        }
    }
}
