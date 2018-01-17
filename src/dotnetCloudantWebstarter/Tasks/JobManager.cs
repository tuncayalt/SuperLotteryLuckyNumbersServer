using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudantDotNet.Services;

namespace CloudantDotNet.Tasks
{
    public class JobManager : IJobManager
    {
        private List<IJob> jobs;

        ICekilisCloudantService _cekilisService;
        ICouponsCloudantService _couponsService;
        IMilliPiyangoService _mpService;
        IUserCloudantService _userService;
        IPushService _pushService;

        public JobManager(ICekilisCloudantService cekilisService, ICouponsCloudantService couponsService, IMilliPiyangoService mpService, IUserCloudantService userService, IPushService pushService)
        {
            _cekilisService = cekilisService;
            _mpService = mpService;
            _couponsService = couponsService;
            _userService = userService;
            _pushService = pushService;

            jobs = new List<IJob>();
            AddCekilisJob();

            CallJobsRepeatedly(TimeSpan.FromSeconds(60));
        }

        private void AddCekilisJob()
        {
            CekilisJob cekilisJob = new CekilisJob(_cekilisService, _mpService);
            cekilisJob.onYeniCekilisFinished += YeniCekilisInvoked;
            AddJob(cekilisJob);
        }

        private void YeniCekilisInvoked(object sender, CekilisEventArgs e)
        {
            AddCouponUpdateJob();
        }

        private void AddCouponUpdateJob()
        {
            CouponUpdateJob couponUpdateJob = new CouponUpdateJob(_cekilisService, _userService, _pushService, _couponsService);
            UpdateCouponEventArgs args = new UpdateCouponEventArgs();
            args.job = couponUpdateJob;
            couponUpdateJob.onCouponUpdateFinished += CouponUpdateFinishedInvoked;
            AddJob(couponUpdateJob);
        }

        private void AddCekilisPushJob()
        {
            CekilisPushJob cekilisPushJob = new CekilisPushJob(_cekilisService, _userService, _pushService);
            PushCekilisEventArgs args = new PushCekilisEventArgs();
            args.job = cekilisPushJob;
            cekilisPushJob.onCekilisPushFinished += CekilisPushFinishedInvoked;
            AddJob(cekilisPushJob);
        }

        private void CouponUpdateFinishedInvoked(object sender, UpdateCouponEventArgs e)
        {
            RemoveJob(e.job);
            AddCekilisPushJob();
        }

        
        private void CekilisPushFinishedInvoked(object sender, PushCekilisEventArgs e)
        {
            RemoveJob(e.job);
            AddCouponPushJob();
        }

        private void CouponPushFinishedInvoked(object sender, PushCouponEventArgs e)
        {
            RemoveJob(e.job);
        }

        private void AddCouponPushJob()
        {
            CouponPushJob couponPushJob = new CouponPushJob(_cekilisService, _userService, _pushService, _couponsService);
            couponPushJob.onCouponPushFinished += CouponPushFinishedInvoked;
            AddJob(couponPushJob);
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
            foreach (IJob job in jobs.ToList())
            {
                if (JobWillStart(job))
                {
                    job.StartJob();
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
