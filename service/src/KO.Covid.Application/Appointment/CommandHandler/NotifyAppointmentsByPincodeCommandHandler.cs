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

    public class NotifyAppointmentsByPincodeCommandHandler
        : IRequestHandler<NotifyAppointmentsByPincodeCommand, bool>
    {
        private readonly IMediator mediator = null;
        private readonly INotifier notifier = null;

        public NotifyAppointmentsByPincodeCommandHandler(
            IMediator mediator,
            INotifier notifier)
        {
            this.mediator = mediator;
            this.notifier = notifier;
        }

        public async Task<bool> Handle(
            NotifyAppointmentsByPincodeCommand request,
            CancellationToken cancellationToken)
        {
            var activeSubscribers = await this.mediator.Send(
                new GetActiveSubscribersQuery());
            if (activeSubscribers.IsNullOrEmpty())
            {
                return true;
            }

            foreach (var subscriber in activeSubscribers)
            {
                if (request.ShouldClearNotifications)
                {
                    subscriber.LastNotifiedCenters.Clear();
                }

                var appointments = await this.GetAppointmentsAsync(subscriber, request.Date);
                var notification = Notification.GetAppointmentNotification(appointments, subscriber);
                if (string.IsNullOrWhiteSpace(notification.Message))
                {
                    continue;
                }

                await this.notifier.SendAsync(
                    subject: "Vaccination centers are now available for booking",
                    recepients: new List<string> { subscriber.Email },
                    message: notification.Message);

                subscriber.LastNotifiedCenters =
                    subscriber.LastNotifiedCenters.AddRange(notification.Centers);

                await this.mediator.Send(
                    new UpdateSubscriberCommand { Subscriber = subscriber });
            }

            return true;
        }

        private async Task<List<AppointmentCalendarResponse>> GetAppointmentsAsync(
            Subscriber subscriber,
            string date)
        {
            var appointments = new List<AppointmentCalendarResponse>();
            foreach (var pincode in subscriber.Pincodes)
            {
                appointments.Add(await this.mediator.Send(
                    new GetAppointmentsCalendarByPincodeQuery
                    {
                        Date = date,
                        Mobile = subscriber.Mobile,
                        Pincode = pincode
                    }));
            }

            return appointments;
        }
    }
}
