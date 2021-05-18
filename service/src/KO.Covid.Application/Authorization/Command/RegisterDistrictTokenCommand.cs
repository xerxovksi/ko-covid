namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class RegisterDistrictTokenCommand : IRequest<bool>
    {
        public string InternalDistrictToken { get; set; }
    }
}
