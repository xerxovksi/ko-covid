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

    public class RemoveInactiveUsersCommandHandler
        : IRequestHandler<RemoveInactiveUsersCommand, bool>
    {
        private readonly ICache<Dictionary<string, DateTime>> activeCache = null;

        public RemoveInactiveUsersCommandHandler(ICache<Dictionary<string, DateTime>> activeCache) =>
            this.activeCache = activeCache;

        public async Task<bool> Handle(
            RemoveInactiveUsersCommand request,
            CancellationToken cancellationToken)
        {
            var currentTime = DateTime.Now;

            var activeUsers = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            if (activeUsers.IsNullOrEmpty())
            {
                return true;
            }

            var inactiveUsers = activeUsers
                .Where(item => DateTime.Now > item.Value)
                .Select(item => item.Key)
                .ToList();

            var isSuccessful = true;
            foreach (var user in inactiveUsers)
            {
                isSuccessful &= await this.activeCache.RemoveAsync(user);
            }

            return isSuccessful;
        }
    }
}
