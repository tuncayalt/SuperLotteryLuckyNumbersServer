using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public class CekilisJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Friday };
        public int startHour { get; set; } = 10;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(10);
        public DateTime lastWorked { get; set; }


        public void StartJob()
        {


            Console.WriteLine("CekilisJob calisti:" + DateTime.UtcNow.GetTurkeyTime());
        }
    }
}
