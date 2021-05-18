namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Domain.Entities;
    using MediatR;

    public class SubscriberCommand : IRequest<Subscriber>
    {
        public Subscriber Subscriber { get; set; }
    }
}
