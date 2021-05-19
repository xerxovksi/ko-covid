namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Authorization;
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
        private readonly IRepository<Subscriber> repository = null;
        private readonly IMediator mediator = null;

        public GetActiveSubscribersQueryHandler(
            IRepository<Subscriber> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public async Task<List<Subscriber>> Handle(
            GetActiveSubscribersQuery request,
            CancellationToken cancellationToken)
        {
            var activeUsers = await this.mediator.Send(
                new GetActiveUsersQuery());

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
