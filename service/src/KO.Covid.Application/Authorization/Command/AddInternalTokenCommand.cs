namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class AddInternalTokenCommand : IRequest<bool>
    {
        public string InternalToken { get; set; }
    }
}
