namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class UpdateSubscriberCommandHandler
        : IRequestHandler<UpdateSubscriberCommand, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;

        public UpdateSubscriberCommandHandler(IRepository<Subscriber> repository) =>
            this.repository = repository;

        public async Task<Subscriber> Handle(
            UpdateSubscriberCommand request,
            CancellationToken cancellationToken) =>
            await this.repository.UpdateItemAsync(request.Subscriber.AddId());
    }
}
