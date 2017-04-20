using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudantDotNet.Services;

namespace CloudantDotNet.Tasks
{
    public class JobManager : IJobManager
    {
        private List<IJob> jobs;

        ICekilisCloudantService _cloudantService;
        ICouponsCloudantService _couponsService;
        IMilliPiyangoService _mpService;
        IUserCloudantService _userService;
        IPushService _pushService;

        public JobManager(ICekilisCloudantService cloudantService, ICouponsCloudantService couponsService, IMilliPiyangoService mpService, IUserCloudantService userService, IPushService pushService)
        {
            _cloudantService = cloudantService;
            _mpService = mpService;
            _couponsService = couponsService;
            _userService = userService;
            _pushService = pushService;

            jobs = new List<IJob>();

            CekilisJob cekilisJob = new CekilisJob(_cloudantService, _mpService);
            cekilisJob.onYeniCekilis += YeniCekilisInvoked;
            AddJob(cekilisJob);

            CallJobsRepeatedly(TimeSpan.FromSeconds(60));
        }

        private void YeniCekilisInvoked(object sender, CekilisEventArgs e)
        {
            CekilisPushJob cekilisPushJob = new CekilisPushJob(_cloudantService, _userService, _pushService);
            PushCekilisEventArgs args = new PushCekilisEventArgs();
            args.job = cekilisPushJob;
            cekilisPushJob.onCekilisPushFinished += CekilisPushFinishedInvoked;
            AddJob(cekilisPushJob);
        }

        private void CekilisPushFinishedInvoked(object sender, PushCekilisEventArgs e)
        {
            RemoveJob(e.job);
        }

        private void CallJobsRepeatedly(TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    StartJobs();
                    await Task.Delay(interval);
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

        public void AddJob(IJob job)
        {
            jobs.Add(job);
        }

        public void RemoveJob(IJob job)
        {
            jobs.Remove(job);
        }
    }
}
