namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Models;
    using KO.Covid.Application.Authorization;
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
        public async Task<IActionResult> GenerateOtpAsync(string mobile) =>
            await this.mediator.SendAsync(
                new GenerateOtpCommand
                {
                    Mobile = mobile
                });

        [HttpGet]
        [Route("confirmotp/{mobile}/{otp}")]
        public async Task<IActionResult> ConfirmOtpAsync(string mobile, string otp) =>
            await this.mediator.SendAsync(
                new ConfirmOtpCommand
                {
                    Mobile = mobile,
                    Otp = otp
                });

        [HttpPost]
        [Route("register/district/token")]
        public async Task<IActionResult> RegisterDistrictTokenAsync(
            [FromBody] RegisterDistrictTokenRequest request) =>
            await this.mediator.SendAsync(
                request: new RegisterDistrictTokenCommand
                {
                    InternalDistrictToken = request.InternalDistrictToken
                },
                successLogMessage: _ => "Successfully registered District token.");
    }
}
