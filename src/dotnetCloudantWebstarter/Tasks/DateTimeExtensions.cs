using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
