namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class RemoveInactiveUsersCommand : IRequest<bool>
    {
    }
}
