namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetAppointmentsCalendarByPincodeQuery
        : IRequest<AppointmentCalendarResponse>
    {
        public string Pincode { get; set; }

        public string Date { get; set; }

        public string Mobile { get; set; }
    }
}
