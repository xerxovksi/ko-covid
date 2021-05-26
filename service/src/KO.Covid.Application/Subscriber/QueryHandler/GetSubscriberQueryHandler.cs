namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Exceptions;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetSubscriberQueryHandler
        : IRequestHandler<GetSubscriberQuery, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;
        private readonly IMediator mediator = null;

        public GetSubscriberQueryHandler(
            IRepository<Subscriber> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public async Task<Subscriber> Handle(
            GetSubscriberQuery request,
            CancellationToken cancellationToken)
        {
            var credential = await this.mediator.Send(
                new GetCredentialQuery { Mobile = request.Mobile });
            if (credential == null
                || string.IsNullOrWhiteSpace(credential.Token))
            {
                throw new AuthorizationException(
                    $"Could not authorize user with mobile: {request.Mobile}.");
            }

            return (await this.repository.GetItemsAsync(
                item => request.Mobile == item.Mobile))?.FirstOrDefault();
        }
    }
}
