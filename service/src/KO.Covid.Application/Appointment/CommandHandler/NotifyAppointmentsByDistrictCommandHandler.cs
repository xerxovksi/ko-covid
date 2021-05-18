namespace KO.Covid.Application.Appointment
{
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

        public NotifyAppointmentsByDistrictCommandHandler(
            IMediator mediator,
            INotifier notifier)
        {
            this.mediator = mediator;
            this.notifier = notifier;
        }

        public async Task<List<string>> Handle(
            NotifyAppointmentsByDistrictCommand request,
            CancellationToken cancellationToken)
        {
            var activeSubscribers = await this.mediator.Send(
                new GetActiveSubscribersQuery());
            if (activeSubscribers.IsNullOrEmpty())
            {
                return default;
            }

            var notifiedSubscribers = new List<string>();
            foreach (var subscriber in activeSubscribers)
            {
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
                            Date = date,
                            Mobile = subscriber.Mobile
                        }));
            }

            return appointments;
        }
    }
}
