namespace KO.Covid.Application
{
    using KO.Covid.Application.Contracts;
    using MediatR;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class EventMediator : IEventMediator<bool>
    {
        private readonly IMediator mediator = null;
        private readonly ITelemetryLogger<EventMediator> logger = null;

        public EventMediator(IMediator mediator, ITelemetryLogger<EventMediator> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> SendAsync<TRequest>(IRequest<TRequest> request)
        {
            try
            {
                var result = await this.mediator.Send(request);
                return true;
            }
            catch (Exception exception)
            {
                this.logger.LogError(
                    exception,
                    "Encountered unexpected Exception for request: {request}, with message: {errorMessage}",
                    JsonConvert.SerializeObject(request),
                    exception.Message);

                return false;
            }
        }
    }
}
