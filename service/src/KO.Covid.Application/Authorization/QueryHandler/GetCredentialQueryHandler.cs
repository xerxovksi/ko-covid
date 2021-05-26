namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Models;
    using KO.Covid.Domain;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetCredentialQueryHandler
        : IRequestHandler<GetCredentialQuery, Credential>
    {
        private readonly ICache<Credential> credentialCache = null;

        public GetCredentialQueryHandler(ICache<Credential> credentialCache) =>
            this.credentialCache = credentialCache;

        public async Task<Credential> Handle(
            GetCredentialQuery request,
            CancellationToken cancellationToken) =>
            await this.credentialCache.GetAsync(
                request.Mobile,
                result => result.FromJson<Credential>());
    }
}
