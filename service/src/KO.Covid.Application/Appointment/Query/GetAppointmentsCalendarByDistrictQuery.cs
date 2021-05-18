namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetAppointmentsCalendarByDistrictQuery : IRequest<AppointmentCalendarResponse>
    {
        public string StateName { get; set; }

        public string DistrictName { get; set; }

        public string Date { get; set; }

        public string Mobile { get; set; }
    }
}
