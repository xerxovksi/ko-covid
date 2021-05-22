namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class AddPublicTokenCommand : IRequest<bool>
    {
        public string PublicToken { get; set; }
    }
}
