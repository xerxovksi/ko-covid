namespace KO.Covid.Subscriber.Functions
{
    using KO.Covid.Application.Contracts;
    using Microsoft.Azure.WebJobs;
    using System.Threading.Tasks;

    public class AppointmentByPincodeSubscriber
    {
        private readonly IEventMediator<bool> mediator = null;

        public AppointmentByPincodeSubscriber(IEventMediator<bool> mediator) =>
            this.mediator = mediator;

        [FunctionName("AppointmentByPincodeSubscriber")]
        public async Task Run(
            [TimerTrigger("%SUBSCRIBER_TIMER%")] TimerInfo timer)
        {

        }
    }
}
