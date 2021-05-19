namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class CreateSubscriberCommandHandler
        : IRequestHandler<CreateSubscriberCommand, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;
        private readonly IMediator mediator = null;

        public CreateSubscriberCommandHandler(
            IRepository<Subscriber> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public async Task<Subscriber> Handle(
            CreateSubscriberCommand request,
            CancellationToken cancellationToken)
        {
            var subscriber = await this.repository.AddItemAsync(request.Subscriber.AddId());

            await this.mediator.Send(
                new AddActiveUserCommand { Mobile = subscriber.Mobile });

            return subscriber;
        }
    }
}
