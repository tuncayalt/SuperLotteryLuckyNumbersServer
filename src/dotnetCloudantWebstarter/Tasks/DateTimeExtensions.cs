using System;

namespace CloudantDotNet.Tasks
{
    public static class DateTimeExtensions
    {
        public static DateTime GetTurkeyTime(this DateTime time)
        {
            DateTime newTime = time.AddHours(3);
            try
            {
                TimeZoneInfo turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                newTime = TimeZoneInfo.ConvertTime(time, turkeyTimeZone);
            }
            catch (Exception)
            {
            }
            return newTime;
        }
    }
}
