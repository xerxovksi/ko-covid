namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class RemoveInactiveTokensCommand : IRequest<bool>
    {
    }
}
