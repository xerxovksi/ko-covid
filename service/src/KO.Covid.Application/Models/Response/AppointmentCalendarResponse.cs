namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain.Entities;
    using System.Collections.Generic;

    public class AppointmentCalendarResponse
    {
        public List<AppointmentCenter> Centers { get; set; }
    }
}
