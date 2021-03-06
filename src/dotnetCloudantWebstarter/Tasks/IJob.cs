﻿using System;

namespace CloudantDotNet.Tasks
{
    public interface IJob
    {
        DayOfWeek[] workDay { get; set; }
        int startHour { get; set; }
        int startMin { get; set; }
        int endHour { get; set; }
        int endMin { get; set; }
        DateTime lastWorked { get; set; }
        TimeSpan onceIn { get; set; }
        void StartJob();
    }
}
