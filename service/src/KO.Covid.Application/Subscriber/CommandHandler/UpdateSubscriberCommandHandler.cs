namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class UpdateSubscriberCommandHandler
        : IRequestHandler<UpdateSubscriberCommand, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;
        private readonly IMediator mediator = null;

        public UpdateSubscriberCommandHandler(
            IRepository<Subscriber> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public async Task<Subscriber> Handle(
            UpdateSubscriberCommand request,
            CancellationToken cancellationToken)
        {
            var existingSubscriber = await this.mediator.Send(
                new GetSubscriberQuery { Mobile = request.Subscriber.Mobile });

            if (existingSubscriber == default)
            {
                throw new ArgumentException(
                    $"Could not find an existing subscriber with mobile: {request.Subscriber.Mobile}.");
            }

            var subscriberToUpdate = request.Subscriber.AddId();
            
            subscriberToUpdate.NotifiedCenters = !request.Subscriber.NotifiedCenters.IsNullOrEmpty()
                ? request.Subscriber.NotifiedCenters
                : existingSubscriber.NotifiedCenters;
            
            subscriberToUpdate.LastNotifiedOn = request.Subscriber.LastNotifiedOn.HasValue
                ? request.Subscriber.LastNotifiedOn
                : existingSubscriber.LastNotifiedOn;

            return await this.repository.UpdateItemAsync(subscriberToUpdate);
        }
    }
}
