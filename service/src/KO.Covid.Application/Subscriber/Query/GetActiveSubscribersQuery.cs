namespace KO.Covid.Application.Subscriber
{
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Collections.Generic;

    public class GetActiveSubscribersQuery : IRequest<List<Subscriber>>
    {
    }
}
