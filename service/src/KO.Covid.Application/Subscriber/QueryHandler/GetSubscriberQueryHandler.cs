namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetSubscriberQueryHandler
        : IRequestHandler<GetSubscriberQuery, Subscriber>
    {
        private readonly IRepository<Subscriber> repository = null;

        public GetSubscriberQueryHandler(IRepository<Subscriber> repository) =>
            this.repository = repository;

        public async Task<Subscriber> Handle(
            GetSubscriberQuery request,
            CancellationToken cancellationToken) =>
            (await this.repository.GetItemsAsync(
                item => request.Mobile == item.Mobile))?.FirstOrDefault();
    }
}
