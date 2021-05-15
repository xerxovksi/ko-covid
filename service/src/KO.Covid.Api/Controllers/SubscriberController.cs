namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain.Entities;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class SubscriberController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public SubscriberController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpPost]
        [Route("states/{mobile}")]
        public async Task<IActionResult> SubscribeAsync(Subscriber subscriber) =>
            await this.mediator.SendAsync(
                new CreateSubscriberCommand
                {
                    Subscriber = new Subscriber
                    {
                        Mobile = subscriber.Mobile,
                        Email = subscriber.Email,
                        Pincodes = subscriber.Pincodes,
                        Districts = subscriber.Districts
                    }
                });
    }
}
