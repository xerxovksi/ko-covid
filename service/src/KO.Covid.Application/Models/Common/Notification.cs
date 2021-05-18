namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain;
    using KO.Covid.Domain.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class Notification
    {
        public HashSet<int> Centers { get; set; }

        public string Message { get; set; }

        public static Notification GetAppointmentNotification(
            List<AppointmentCalendarResponse> appointments,
            Subscriber subscriber)
        {
            var centers = appointments.SelectMany(calendar => calendar.Centers);
            var availableCenters = centers
                .Where(center => center.IsAvailable(subscriber.Age ?? 0) && center.CenterId.HasValue)
                .ToList();

            if (availableCenters.IsNullOrEmpty())
            {
                return new Notification
                {
                    Centers = subscriber.LastNotifiedCenters,
                    Message = string.Empty
                };
            }

            var newAvailableCenters = availableCenters
                .Where(center => !subscriber.LastNotifiedCenters.Contains(center.CenterId.Value))
                .ToList();

            if (newAvailableCenters.IsNullOrEmpty())
            {
                return new Notification
                {
                    Centers = subscriber.LastNotifiedCenters,
                    Message = string.Empty
                };
            }

            var centersCount = newAvailableCenters.Count;
            var message = new StringBuilder(
                $"<div width='100%'><h2>{centersCount} vaccination center(s) are now available for booking</h2>");

            for (var i = 0; i < centersCount; i++)
            {
                var center = newAvailableCenters[i];
                message.Append(
                    $"<b>{i + 1}. {center.Name}</b><br />{center.BlockName}, {center.Address}<br />");
                message.Append(
                    $"{center.DistrictName}, {center.StateName} - {center.Pincode}.<br />");
                message.Append(
                    $"<b>Fee:</b> {center.FeeType}<br /><b>Operational Time:</b> {center.From} to {center.To}<br />");

                var sessionsCount = center.Sessions.Count;
                if (sessionsCount <= 0)
                {
                    continue;
                }
                
                for (var j = 0; j < sessionsCount; j++)
                {
                    var session = center.Sessions[j];
                    message.Append(
                        $"<b>Date: </b>{session.Date}&emsp;<b>Vaccine: </b>{session.Vaccine}&emsp;<b>Minimum Age: </b>{session.MinimumAgeLimit}&emsp;");
                    message.Append(
                        $"<b>Dose 1: </b>{session.AvaialableCapacityDose1}&emsp;<b>Dose 2: </b>{session.AvaialableCapacityDose2}<br />");
                }

                message.Append("<br />");
            }

            message.Append("</div>");

            return new Notification
            {
                Centers = newAvailableCenters.Select(center => center.CenterId.Value).ToHashSet(),
                Message = message.ToString()
            };
        }
    }
}
