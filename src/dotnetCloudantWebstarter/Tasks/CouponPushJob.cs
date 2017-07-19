using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudantDotNet.Services;
using CloudantDotNet.Models;

namespace CloudantDotNet.Tasks
{
    public class CouponPushJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday };
        public int startHour { get; set; } = 18;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(1);
        public DateTime lastWorked { get; set; }


        private ICekilisCloudantService _cekilisService;
        private ICouponsCloudantService _couponsService;
        private IPushService _pushService;
        private IUserCloudantService _userService;

        public event EventHandler<PushCouponEventArgs> onCouponPushFinished;

        public CouponPushJob(ICekilisCloudantService _cekilisService, IUserCloudantService _userService, IPushService _pushService, ICouponsCloudantService _couponsService)
        {
            this._cekilisService = _cekilisService;
            this._userService = _userService;
            this._pushService = _pushService;
            this._couponsService = _couponsService;
        }

        public void StartJob()
        {
            SendCouponPushToUsers();

            Console.WriteLine("CouponPushJob ran:" + DateTime.UtcNow.GetTurkeyTime());
        }

        private async void SendCouponPushToUsers()
        {
            Cekilis cekilis = await _cekilisService.GetAsync();
            if (cekilis == null)
                return;

            List<User> userList = await _userService.GetPushCekilis();
            if (userList == null || !userList.Any())
            {
                CouponPushFinished();
                return;
            }

            foreach (User user in userList)
            {
                List<CouponDto> couponList = await _couponsService.GetAllByUserNameAndTarih(user.user_id, cekilis.tarih);
                if (couponList == null || !couponList.Any())
                    continue;
                if (!couponList.Any(c => c.WinCount >= 3))
                    continue;

                int maxWinCount = couponList.Max(c => c.WinCount);
                PushNotification push = PushNotificationCoupon.Build(maxWinCount, cekilis.tarih_view, user.token);
                try
                {
                    await _pushService.SendPush(push);
                }
                catch (Exception)
                {
                    Console.WriteLine("Cant send push to user:" + user.user_id);
                }

                await Task.Delay(100);
            }

            CouponPushFinished();
        }

        private void CouponPushFinished()
        {
            PushCouponEventArgs args = new PushCouponEventArgs()
            {
                job = this
            };
            onCouponPushFinished.Invoke(this, args);
        }
    }
    public class PushCouponEventArgs : EventArgs
    {
        public IJob job;
    }
}
