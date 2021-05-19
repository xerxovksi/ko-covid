namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using MediatR;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class AddActiveUserCommandHandler
        : IRequestHandler<AddActiveUserCommand, bool>
    {
        private const string ActiveCacheKey = "ActiveUsers";
        private readonly ICache<HashSet<string>> activeCache = null;

        public AddActiveUserCommandHandler(ICache<HashSet<string>> activeCache) =>
            this.activeCache = activeCache;

        public async Task<bool> Handle(
            AddActiveUserCommand request,
            CancellationToken cancellationToken)
        {
            var activeMobiles = await this.activeCache.GetAsync(
                ActiveCacheKey,
                result => result.FromJson<HashSet<string>>());

            if (activeMobiles == default)
            {
                return await this.activeCache.SetAsync(
                    ActiveCacheKey,
                    ActiveCacheDuration,
                    () => new List<string> { request.Mobile }.ToJson());
            }

            if (activeMobiles.Contains(request.Mobile))
            {
                return true;
            }

            activeMobiles.Add(request.Mobile);
            
            return await this.activeCache.SetAsync(
                ActiveCacheKey,
                ActiveCacheDuration,
                () => activeMobiles.ToJson());
        }
    }
}
