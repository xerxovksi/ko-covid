namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    using static KO.Covid.Application.Constants;

    public class RegisterPincodeTokenCommandHandler
        : IRequestHandler<RegisterPincodeTokenCommand, bool>
    {
        private const string TokenCacheKey = "InternalPincodeToken";
        private readonly ICache<string> tokenCache = null;

        public RegisterPincodeTokenCommandHandler(ICache<string> tokenCache) =>
            this.tokenCache = tokenCache;

        public async Task<bool> Handle(
            RegisterPincodeTokenCommand request,
            CancellationToken cancellationToken) =>
            await this.tokenCache.SetAsync(
                TokenCacheKey,
                TokenCacheDuration,
                () => request.InternalPincodeToken);
    }
}
