namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class RegisterPincodeTokenCommand : IRequest<bool>
    {
        public string InternalPincodeToken { get; set; }
    }
}
