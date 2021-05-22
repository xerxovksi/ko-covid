namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class AddActiveUserCommandHandler
        : IRequestHandler<AddActiveUserCommand, bool>
    {
        private const string ActiveCacheKey = "ActiveUsers";
        private readonly ICache<Dictionary<string, DateTime>> activeCache = null;

        public AddActiveUserCommandHandler(ICache<Dictionary<string, DateTime>> activeCache) =>
            this.activeCache = activeCache;

        public async Task<bool> Handle(
            AddActiveUserCommand request,
            CancellationToken cancellationToken)
        {
            var timeToLive = DateTime.Now.Add(ActiveCacheDuration);

            var activeUsers = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<Dictionary<string, DateTime>>());

            if (activeUsers.IsNullOrEmpty())
            {
                return await this.activeCache.SetAsync(
                    ActiveCacheKey,
                    ActiveCacheDuration,
                    () => new Dictionary<string, DateTime>
                    {
                        { request.Mobile, timeToLive }
                    }.ToJson());
            }

            activeUsers[request.Mobile] = timeToLive;
            
            return await this.activeCache.SetAsync(
                ActiveCacheKey,
                ActiveCacheDuration,
                () => activeUsers.ToJson());
        }
    }
}
