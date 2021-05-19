namespace KO.Covid.Application.Authorization
{
    using MediatR;
    using System.Collections.Generic;

    public class GetActiveUsersQuery : IRequest<HashSet<string>>
    {
    }
}
