namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class CreateSubscriberCommandHandler
        : IRequestHandler<CreateSubscriberCommand, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;

        public CreateSubscriberCommandHandler(IRepository<Subscriber> repository) =>
            this.repository = repository;

        public async Task<Subscriber> Handle(
            CreateSubscriberCommand request,
            CancellationToken cancellationToken) =>
            await this.repository.AddItemAsync(request.Subscriber.AddId());
    }
}
