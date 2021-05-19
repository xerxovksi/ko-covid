namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using MediatR;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetActiveUsersQueryHandler
        : IRequestHandler<GetActiveUsersQuery, HashSet<string>>
    {
        private const string ActiveCacheKey = "ActiveUsers";
        private readonly ICache<HashSet<string>> activeCache = null;

        public GetActiveUsersQueryHandler(ICache<HashSet<string>> activeCache) =>
            this.activeCache = activeCache;

        public async Task<HashSet<string>> Handle(
            GetActiveUsersQuery request,
            CancellationToken cancellationToken) =>
            await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<HashSet<string>>());
    }
}
