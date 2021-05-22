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
                request: new GenerateOtpCommand
                {
                    Mobile = mobile
                },
                successLogMessage: _ => "Initiated authorization of user with mobile: {mobile}.",
                successLogParameters: _ => new string[] { mobile });

        [HttpGet]
        [Route("confirmotp/{mobile}/{otp}")]
        public async Task<IActionResult> ConfirmOtpAsync(string mobile, string otp) =>
            await this.mediator.SendAsync(
                request: new ConfirmOtpCommand
                {
                    Mobile = mobile,
                    Otp = otp
                },
                successLogMessage: _ => "Successfully authorized user with mobile: {mobile}.",
                successLogParameters: _ => new string[] { mobile });

        [HttpPost]
        [Route("register/public/token")]
        public async Task<IActionResult> RegisterPublicTokenAsync(
            [FromBody] RegisterTokenRequest request) =>
            await this.mediator.SendAsync(
                request: new AddPublicTokenCommand
                {
                    PublicToken = request.Token
                },
                successLogMessage: _ => "Successfully registered public token.");

        [HttpPost]
        [Route("register/internal/token")]
        public async Task<IActionResult> RegisterInternalTokenAsync(
            [FromBody] RegisterTokenRequest request) =>
            await this.mediator.SendAsync(
                request: new AddInternalTokenCommand
                {
                    InternalToken = request.Token
                },
                successLogMessage: _ => "Successfully registered internal token.");
    }
}
