namespace KO.Covid.Application.Appointment
{
    using MediatR;
    using System.Collections.Generic;

    public class NotifyAppointmentsByDistrictCommand : IRequest<List<string>>
    {
        public string Date { get; set; }

        public bool ShouldClearNotifications { get; set; }

        public bool ShouldClearInactiveUsers { get; set; }
    }
}
