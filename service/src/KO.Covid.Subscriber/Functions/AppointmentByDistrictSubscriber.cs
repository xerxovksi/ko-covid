namespace KO.Covid.Subscriber.Functions
{
    using KO.Covid.Application;
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using Microsoft.Azure.WebJobs;
    using System.Threading.Tasks;

    public class AppointmentByDistrictSubscriber
    {
        private readonly IEventMediator<bool> mediator = null;

        public AppointmentByDistrictSubscriber(IEventMediator<bool> mediator) =>
            this.mediator = mediator;

        [FunctionName("AppointmentByDistrictSubscriber")]
        public async Task Run(
            [TimerTrigger("%SUBSCRIBER_TIMER%")] TimerInfo timer)
        {
            var startDate = timer.GetStartTime().ToIndianDate();
            var endDate = timer.GetEndTime().ToIndianDate();

            await this.mediator.SendAsync(
                request: new NotifyAppointmentsByDistrictCommand
                {
                    Date = startDate,
                    ShouldClearNotifications = !startDate.Equals(endDate),
                    ShouldClearInactiveUsers = !startDate.Equals(endDate)
                },
                successLogMessage: result => result.IsNullOrEmpty() 
                    ? default    
                    : "Successfully notified appointments to subscribers: {notifiedSubscribers}.",
                successLogParameters: result => result.IsNullOrEmpty() 
                    ? default
                    : new string[] { result.ToJson() });
        }
    }
}
