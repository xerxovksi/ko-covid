namespace KO.Covid.Application.Appointment
{
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Models;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class NotifyAppointmentsByDistrictCommandHandler
        : IRequestHandler<NotifyAppointmentsByDistrictCommand, List<string>>
    {
        private readonly IMediator mediator = null;
        private readonly INotifier notifier = null;
        private readonly ITelemetryLogger<NotifyAppointmentsByDistrictCommandHandler> logger = null;

        public NotifyAppointmentsByDistrictCommandHandler(
            IMediator mediator,
            INotifier notifier,
            ITelemetryLogger<NotifyAppointmentsByDistrictCommandHandler> logger)
        {
            this.mediator = mediator;
            this.notifier = notifier;
            this.logger = logger;
        }

        public async Task<List<string>> Handle(
            NotifyAppointmentsByDistrictCommand request,
            CancellationToken cancellationToken)
        {
            var activeSubscribers = await this.mediator.Send(
                new GetActiveSubscribersQuery());
            if (activeSubscribers.IsNullOrEmpty())
            {
                this.logger.LogInformation("No active subscribers found.");
                return default;
            }

            if (request.ShouldCleanUpInactiveResources)
            {
                await this.mediator.Send(new RemoveInactiveUsersCommand());
                await this.mediator.Send(new RemoveInactiveTokensCommand());
            }

            var activeSubscribersCount = activeSubscribers.Count;
            this.logger.LogInformation(
                "Found {activeSubscribersCount} active subscribers.",
                activeSubscribersCount);

            var notifiedSubscribers = new List<string>();
            for (var i = 0; i < activeSubscribersCount; i++)
            {
                var subscriber = activeSubscribers[i];

                // ToDo: Enable this check when
                // the availability of appointments is high.
                //if (request.ShouldClearNotifications)
                //{
                    subscriber.LastNotifiedCenters.Clear();
                //}

                var appointments = await this.GetAppointmentsAsync(subscriber, request.Date);
                var notification = Notification.GetAppointmentNotification(appointments, subscriber);
                if (string.IsNullOrWhiteSpace(notification.Message))
                {
                    continue;
                }

                await this.notifier.SendAsync(
                    subject: "Vaccination center(s) are now available for booking",
                    recepients: new List<string> { subscriber.Email },
                    message: notification.Message);

                notifiedSubscribers.Add(subscriber.Mobile);

                subscriber.LastNotifiedCenters =
                    subscriber.LastNotifiedCenters.AddRange(notification.Centers);

                await this.mediator.Send(
                    new UpdateSubscriberCommand { Subscriber = subscriber });
            }

            return notifiedSubscribers;
        }

        private async Task<List<AppointmentCalendarResponse>> GetAppointmentsAsync(
            Subscriber subscriber,
            string date)
        {
            var appointments = new List<AppointmentCalendarResponse>();
            foreach (var district in subscriber.Districts)
            {
                appointments.Add(
                    await this.mediator.Send(
                        new GetAppointmentsCalendarByDistrictQuery
                        {
                            StateName = district.StateName,
                            DistrictName = district.DistrictName,
                            Date = date
                        }));
            }

            return appointments;
        }
    }
}
