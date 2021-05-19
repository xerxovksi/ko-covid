﻿namespace KO.Covid.Api.Controllers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain.Entities;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api")]
    public class SubscriberController : ControllerBase
    {
        private readonly IRequestMediator mediator = null;

        public SubscriberController(IRequestMediator mediator) =>
            this.mediator = mediator;

        [HttpPost]
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
                        Pincodes = subscriber.Pincodes,
                        Districts = subscriber.Districts,
                        IsActive = true
                    }
                },
                successLogMessage: _ => "Successfully created Subscriber with Mobile: {mobile}.",
                successLogParameters: result => new string[] { result.Mobile });

        [HttpGet]
        [Route("subscribers")]
        public async Task<IActionResult> GetActiveAsync() =>
            await this.mediator.SendAsync(
                new GetActiveSubscribersQuery());

        [HttpPut]
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
                        Pincodes = subscriber.Pincodes,
                        Districts = subscriber.Districts,
                        IsActive = true
                    }
                },
                successLogMessage: _ => "Successfully updated Subscriber with Mobile: {mobile}.",
                successLogParameters: result => new string[] { result.Mobile });
    }
}
