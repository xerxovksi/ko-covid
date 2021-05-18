namespace KO.Covid.Application.Authorization
{
    using MediatR;

    public class GenerateOtpCommand : IRequest<bool>
    {
        public string Mobile { get; set; }
    }
}
