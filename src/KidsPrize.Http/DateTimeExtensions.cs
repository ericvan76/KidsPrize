using System;

namespace KidsPrize.Http
{
    public static class DateTimeExtensions
    {
        public static bool IsCalendarDate(this DateTime date)
        {
            return date.TimeOfDay == TimeSpan.Zero;
        }

        public static bool IsStartOfWeek(this DateTime date)
        {
            return date == date.StartOfWeek();
        }

        public static DateTime StartOfWeek(this DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek).Date;
        }

        public static DateTime StartOfNextWeek(this DateTime date)
        {
            return date.StartOfWeek().AddDays(7);
        }
    }
}