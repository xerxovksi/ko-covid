namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Domain.Entities;
    using MediatR;

    public class CreateSubscriberCommand : IRequest<Subscriber>
    {
        public Subscriber Subscriber { get; set; }
    }
}
