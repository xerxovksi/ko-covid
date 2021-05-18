namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using MediatR;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RegisterDistrictTokenCommandHandler
        : IRequestHandler<RegisterDistrictTokenCommand, bool>
    {
        private const string TokenCacheKey = "InternalDistrictToken";
        private readonly ICache<string> tokenCache = null;

        public RegisterDistrictTokenCommandHandler(ICache<string> tokenCache) =>
            this.tokenCache = tokenCache;

        public async Task<bool> Handle(
            RegisterDistrictTokenCommand request,
            CancellationToken cancellationToken) =>
            await this.tokenCache.SetAsync(
                TokenCacheKey,
                TimeSpan.FromDays(3),
                () => request.InternalDistrictToken);
    }
}
