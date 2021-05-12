namespace KO.Covid.Application.Otp
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GenerateOtpCommand : IRequest<GenerateOtpResponse>
    {
        public string Mobile { get; set; }
    }
}
