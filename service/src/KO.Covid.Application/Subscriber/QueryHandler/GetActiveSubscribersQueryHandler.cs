namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetActiveSubscribersQueryHandler
        : IRequestHandler<GetActiveSubscribersQuery, List<Subscriber>>
    {
        private const string ActiveCacheKey = "ActiveUsers";

        private readonly ICache<HashSet<string>> activeCache = null;
        private readonly IRepository<Subscriber> repository = null;

        public GetActiveSubscribersQueryHandler(
            ICache<HashSet<string>> activeCache,
            IRepository<Subscriber> repository)
        {
            this.activeCache = activeCache;
            this.repository = repository;
        }

        public async Task<List<Subscriber>> Handle(
            GetActiveSubscribersQuery request,
            CancellationToken cancellationToken)
        {
            var activeUsers = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<HashSet<string>>());

            if (activeUsers.IsNullOrEmpty())
            {
                return null;
            }

            var subscribers = await this.repository.GetItemsAsync(
                item => activeUsers.Contains(item.Mobile));

            return subscribers?.ToList();
        }
    }
}
