namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api")]
    public class GeoController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public GeoController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpGet]
        [Route("states/{mobile}")]
        public async Task<IActionResult> GetStatesAsync(string mobile) =>
            await this.mediator.SendAsync(
                new GetStatesQuery
                {
                    Mobile = mobile
                });

        [HttpGet]
        [Route("districts/{mobile}")]
        public async Task<IActionResult> GetDistrictsAsync(
            string mobile,
            [FromQuery] string stateName) =>
            await this.mediator.SendAsync(
                new GetDistrictsQuery
                {
                    StateName = stateName,
                    Mobile = mobile
                });
    }
}
