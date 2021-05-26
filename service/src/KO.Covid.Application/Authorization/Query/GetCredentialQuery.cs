namespace KO.Covid.Application.Authorization
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetCredentialQuery : IRequest<Credential>
    {
        public string Mobile { get; set; }
    }
}
