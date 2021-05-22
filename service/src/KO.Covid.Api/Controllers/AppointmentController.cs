namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api")]
    public class AppointmentController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public AppointmentController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpGet]
        [Route("appointment/bydistrict/{mobile}")]
        public async Task<IActionResult> GetAppointmentAsync(
            string mobile,
            [FromQuery] string stateName,
            [FromQuery] string districtName,
            [FromQuery] string date) =>
            await this.mediator.SendAsync(
                new GetAppointmentsByDistrictQuery
                {
                    StateName = stateName,
                    DistrictName = districtName,
                    Date = date,
                    Mobile = mobile
                });

        [HttpGet]
        [Route("appointment/calendar/bydistrict/{mobile}")]
        public async Task<IActionResult> GetAppointmentCalendarAsync(
            string mobile,
            [FromQuery] string stateName,
            [FromQuery] string districtName,
            [FromQuery] string date) =>
            await this.mediator.SendAsync(
                new GetAppointmentsCalendarByDistrictQuery
                {
                    StateName = stateName,
                    DistrictName = districtName,
                    Date = date,
                    Mobile = mobile
                });
    }
}
