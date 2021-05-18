namespace KO.Covid.Application.Appointment
{
    using MediatR;

    public class NotifyAppointmentsByPincodeCommand : IRequest<bool>
    {
        public string Date { get; set; }

        public bool ShouldClearNotifications { get; set; }
    }
}
