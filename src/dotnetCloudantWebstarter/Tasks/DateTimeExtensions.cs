using System;

namespace CloudantDotNet.Tasks
{
    public static class DateTimeExtensions
    {
        public static DateTime GetTurkeyTime(this DateTime time)
        {
            return time.AddHours(3);
        }
    }
}
