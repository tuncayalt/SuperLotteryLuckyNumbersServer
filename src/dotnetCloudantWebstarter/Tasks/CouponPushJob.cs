using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudantDotNet.Services;
using CloudantDotNet.Models;
using dotnetCloudantWebstarter.Cache;

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
            //SendCouponPushToUsers().Wait();
            SendPushToWinners().Wait();

            Console.WriteLine("CouponPushJob ran:" + DateTime.UtcNow.GetTurkeyTime());
        }

        private async Task SendPushToWinners()
        {
            try
            {
                Cekilis cekilis = CekilisCache.cekilisList.Last();
                if (cekilis == null)
                    return;

                List<Coupon> winnerCouponList = await _couponsService.GetAllByWinCountAndTarih(cekilis.tarih);
                if (winnerCouponList == null || !winnerCouponList.Any())
                {
                    CouponPushFinished();
                    return;
                }
                Dictionary<string, int> userDict = new Dictionary<string, int>();
                foreach (Coupon item in winnerCouponList)
                {
                    if (!userDict.ContainsKey(item.User) || userDict[item.User] < item.WinCount) {
                        userDict[item.User] = item.WinCount;
                    }
                }

                List<User> winnerUserList = await _userService.GetAllByUserIds(userDict.Keys.ToList());
                if (winnerUserList == null || !winnerUserList.Any())
                {
                    CouponPushFinished();
                    return;
                }

                var winnerUsersToPushList =  winnerUserList.Where(u => u.push_cekilis.Equals("T")).ToList();
                if (winnerUsersToPushList == null || !winnerUsersToPushList.Any())
                {
                    CouponPushFinished();
                    return;
                }

                foreach (var user in winnerUsersToPushList)
                {
                    try
                    {
                        PushNotification push = PushNotificationCoupon.Build(userDict[user.user_id], cekilis.tarih_view, user.token);

                        bool pushResult = await _pushService.SendPush(push);
                        if (!pushResult)
                        {
                            user.push_cekilis = "F";
                            user.push_win = "F";
                            await _userService.UpdateAsync(user);
                            Console.WriteLine("CouponPushJob winner: " + user.user_id + " won:" + userDict[user.user_id]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CouponPushJob.SendPushToWinners, user:" + user.user_id + " " + ex.StackTrace);
                    }
                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("CouponPushJob.SendPushToWinners hata. " + ex.StackTrace);
            }
            CouponPushFinished();
        }

        private async Task SendCouponPushToUsers()
        {
            try
            {
                Cekilis cekilis = CekilisCache.cekilisList.Last();
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
                    try
                    {
                        List<Coupon> couponList = await _couponsService.GetAllByUserNameAndTarih(user.user_id, cekilis.tarih);
                        if (couponList == null || !couponList.Any())
                            continue;
                        if (!couponList.Any(c => c.WinCount >= 3))
                            continue;

                        int maxWinCount = couponList.Max(c => c.WinCount);
                        PushNotification push = PushNotificationCoupon.Build(maxWinCount, cekilis.tarih_view, user.token);

                        bool pushResult = await _pushService.SendPush(push);
                        if (!pushResult)
                        {
                            user.push_cekilis = "F";
                            user.push_win = "F";
                            await _userService.UpdateAsync(user);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CouponPushJob.SendCouponPushToUsers, user:" + user.user_id + " " + ex.StackTrace);
                    }

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CouponPushJob.SendCouponPushToUsers hata. " + ex.StackTrace);
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
