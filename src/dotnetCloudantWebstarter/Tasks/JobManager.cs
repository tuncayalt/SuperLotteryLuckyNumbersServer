using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudantDotNet.Services;

namespace CloudantDotNet.Tasks
{
    public class JobManager
    {
        List<IJob> jobs;

        public JobManager(ICekilisCloudantService cloudantService, ICouponsCloudantService couponsService, IMilliPiyangoService mpService)
        {

            jobs = new List<IJob>();

            CekilisJob cekilisJob = new CekilisJob(cloudantService, mpService);
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

        private void StartJobs()
        {
            foreach (IJob job in jobs)
            {
                if (JobWillStart(job))
                {
                    Task jobTask = new Task(job.StartJob);
                    jobTask.Start();
                    job.lastWorked = DateTime.UtcNow.GetTurkeyTime();
                }

            }
        }

        private bool JobWillStart(IJob job)
        {
            return DayOk(job) && StartTimeOk(job) && EndTimeOk(job) && OnceInOk(job);
        }

        private bool OnceInOk(IJob job)
        {
            if (job.lastWorked > DateTime.UtcNow.GetTurkeyTime() - job.onceIn)
                return false;
            return true;
        }

        private bool EndTimeOk(IJob job)
        {
            return job.endHour >= DateTime.UtcNow.GetTurkeyTime().Hour && job.endMin >= DateTime.UtcNow.Minute;
        }

        private bool StartTimeOk(IJob job)
        {
            return job.startHour <= DateTime.UtcNow.GetTurkeyTime().Hour && job.startMin <= DateTime.UtcNow.Minute;
        }

        private static bool DayOk(IJob job)
        {
            return job.workDay.Contains(DateTime.Now.GetTurkeyTime().DayOfWeek);
        }
    }
}
