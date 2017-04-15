using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public class JobManager
    {
        List<IJob> jobs;

        public JobManager()
        {
            jobs = new List<IJob>();
            CekilisJob cekilisJob = new CekilisJob();
            jobs.Add(cekilisJob);

            CallJobsRepeatedly(TimeSpan.FromSeconds(60));
        }

        private void CallJobsRepeatedly(TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);
                    StartJobs();
                }
            });
        }

        //private void AddCache()
        //{
        //    result = cache.Set(
        //                    key,
        //                    new object(),
        //                    new MemoryCacheEntryOptions()
        //                    .SetPriority(CacheItemPriority.NeverRemove)
        //                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(1))
        //                    .RegisterPostEvictionCallback(StartJobs)
        //                    );
        //}

        public void StartJobs()
        {
            foreach (IJob job in jobs)
            {
                if (JobWillStart(job))
                {
                    Task jobTask = new Task(job.StartJob);
                    jobTask.Start();
                }

            }
        }

        private bool JobWillStart(IJob job)
        {
            return DayOk(job) && StartTimeOk(job) && EndTimeOk(job);
        }

        private bool EndTimeOk(IJob job)
        {
            return job.endHour >= DateTime.UtcNow.Hour && job.endMin >= DateTime.UtcNow.Minute;
        }

        private bool StartTimeOk(IJob job)
        {
            return job.startHour <= DateTime.UtcNow.Hour && job.startMin <= DateTime.UtcNow.Minute;
        }

        private static bool DayOk(IJob job)
        {
            return job.workDay.Contains(DateTime.Today.DayOfWeek);
        }
    }
}
