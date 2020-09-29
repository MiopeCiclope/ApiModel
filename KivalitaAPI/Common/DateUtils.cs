using System;

namespace KivalitaAPI.Common
{
    public class DateUtils
    {

        public static bool IsDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }

        public static DateTime GetBusinessDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date.AddDays(2);
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return date.AddDays(1);
            }

            return date;
        }

    }
}
