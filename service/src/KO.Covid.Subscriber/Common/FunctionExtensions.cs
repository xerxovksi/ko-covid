namespace KO.Covid.Subscriber
{
    using Microsoft.Azure.WebJobs;
    using System;

    public static class FunctionExtensions
    {
        public static DateTime GetStartTime(this TimerInfo timer)
        {
            var last = timer.ScheduleStatus.Last;
            if (last.Equals(DateTime.MinValue) == false)
            {
                return last;
            }

            var next = timer.ScheduleStatus.Next;
            var later = timer.Schedule.GetNextOccurrence(next);

            return next.Add((later - next) * -1);
        }

        public static DateTime GetEndTime(this TimerInfo timer) =>
            timer.ScheduleStatus.Next;
    }
}
