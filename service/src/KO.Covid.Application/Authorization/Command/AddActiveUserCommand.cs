namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class AddActiveUserCommand : IRequest<bool>
    {
        public string Mobile { get; set; }
    }
}
