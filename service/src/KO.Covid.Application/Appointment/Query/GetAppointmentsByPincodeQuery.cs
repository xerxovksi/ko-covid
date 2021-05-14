namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetAppointmentsByPincodeQuery : IRequest<AppointmentResponse>
    {
        public string Pincode { get; set; }

        public string Date { get; set; }

        public string Mobile { get; set; }
    }
}
