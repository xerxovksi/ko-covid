namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetAppointmentsByDistrictQuery : IRequest<AppointmentResponse>
    {
        public string StateName { get; set; }

        public string DistrictName { get; set; }

        public string Date { get; set; }

        public string PublicToken { get; set; }
    }
}
