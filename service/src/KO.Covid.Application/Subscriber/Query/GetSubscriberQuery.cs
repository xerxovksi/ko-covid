namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Domain.Entities;
    using MediatR;

    public class GetSubscriberQuery : IRequest<Subscriber>
    {
        public string Mobile { get; set; }
    }
}
