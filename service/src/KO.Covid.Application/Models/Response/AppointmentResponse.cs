namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain.Entities;
    using System.Collections.Generic;

    public class AppointmentResponse
    {
        public List<Appointment> Sessions { get; set; }
    }
}
