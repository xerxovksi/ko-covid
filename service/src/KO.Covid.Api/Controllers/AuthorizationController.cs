namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Otp;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public AuthorizationController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpGet]
        [Route("generateotp/{mobile}")]
        public async Task<IActionResult> GetAllAsync(string mobile) =>
            await this.mediator.SendAsync(new GenerateOtpCommand { Mobile = mobile });
    }
}
