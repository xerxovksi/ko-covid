namespace KO.Covid.Application.Otp
{
    using MediatR;

    public class ConfirmOtpCommand : IRequest<bool>
    {
        public string Mobile { get; set; }

        public string Otp { get; set; }
    }
}
