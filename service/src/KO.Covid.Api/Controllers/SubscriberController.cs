namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api")]
    public class SubscriberController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public SubscriberController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpGet]
        [Authorize(Policy = "ShouldBeSignedIn")]
        [Route("subscribers")]
        public async Task<IActionResult> GetAsync([FromQuery] string mobile) =>
            await this.mediator.SendAsync(
                new GetSubscriberQuery { Mobile = mobile });

        [HttpGet]
        [Route("subscribers/active")]
        public async Task<IActionResult> GetActiveAsync() =>
            await this.mediator.SendAsync(
                new GetActiveSubscribersQuery());

        [HttpPost]
        [Authorize(Policy = "ShouldBeSignedIn")]
        [Route("subscribers")]
        public async Task<IActionResult> CreateAsync(Subscriber subscriber) =>
            await this.mediator.SendAsync(
                request: new CreateSubscriberCommand
                {
                    Subscriber = new Subscriber
                    {
                        Name = subscriber.Name,
                        Mobile = subscriber.Mobile,
                        Email = subscriber.Email,
                        Age = subscriber.Age,
                        Districts = subscriber.Districts,
                        IsActive = subscriber.IsActive ?? false
                    }
                },
                successLogMessage: _ => "Successfully created subscriber with mobile: {mobile}.",
                successLogParameters: result => new string[] { result.Mobile });

        [HttpPut]
        [Authorize(Policy = "ShouldBeSignedIn")]
        [Route("subscribers")]
        public async Task<IActionResult> UpdateAsync(Subscriber subscriber) =>
            await this.mediator.SendAsync(
                request: new UpdateSubscriberCommand
                {
                    Subscriber = new Subscriber
                    {
                        Name = subscriber.Name,
                        Mobile = subscriber.Mobile,
                        Email = subscriber.Email,
                        Age = subscriber.Age,
                        Districts = subscriber.Districts,
                        IsActive = subscriber.IsActive ?? false
                    }
                },
                successLogMessage: _ => "Successfully updated subscriber with mobile: {mobile}.",
                successLogParameters: result => new string[] { result.Mobile });
    }
}
