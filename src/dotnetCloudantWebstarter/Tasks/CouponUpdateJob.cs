﻿using CloudantDotNet.Models;
using CloudantDotNet.Services;
using dotnetCloudantWebstarter.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public class CouponUpdateJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday };
        public int startHour { get; set; } = 18;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(1);
        public DateTime lastWorked { get; set; }
        public int updateCouponCount { get; set; } = 600;

        ICekilisCloudantService _cekilisService;
        IUserCloudantService _userService;
        IPushService _pushService;
        ICouponsCloudantService _couponService;

        public event EventHandler<UpdateCouponEventArgs> onCouponUpdateFinished;

        public CouponUpdateJob(ICekilisCloudantService cekilisService, IUserCloudantService userService, IPushService pushService, ICouponsCloudantService couponService)
        {
            _cekilisService = cekilisService;
            _userService = userService;
            _pushService = pushService;
            _couponService = couponService;
        }

        public void StartJob()
        {
            UpdateUserWinCounts().Wait();

            Console.WriteLine("CouponUpdateJob ran:" + DateTime.UtcNow.GetTurkeyTime());
        }

        private async Task UpdateUserWinCounts()
        {
            Cekilis cekilis = CekilisCache.cekilisList.Last();
            if (cekilis == null)
                return;

            /*
            List<User> userList = await _userService.GetPushCekilis();
            if (userList == null || !userList.Any())
            {
                CouponUpdateFinished();
                return;
            }
            */
            List<CouponDto> couponList = await _couponService.GetWithLimitByTarih(cekilis.tarih, updateCouponCount);
            if (couponList == null || !couponList.Any())
            {
                CouponUpdateFinished();
                return;
            }

            foreach (CouponDto couponDto in couponList)
            {
                int winCount = GetWinCount(couponDto, cekilis);
                couponDto.WinCount = winCount;
            }
            CouponListDto couponListDto = new CouponListDto();
            couponListDto.docs = couponList;
            await _couponService.UpdateBulkAsync(couponListDto);

            /*
            foreach (var user in userList)
            {
                List<CouponDto> userCouponList = couponList.Where(c => c.User.Equals(user.user_id)).ToList();
                if (userCouponList == null || !userCouponList.Any())
                {
                    continue;
                }

                foreach (CouponDto couponDto in userCouponList)
                {
                    int winCount = GetWinCount(couponDto, cekilis);
                    couponDto.WinCount = winCount;
                }

                CouponListDto couponListDto = new CouponListDto();
                couponListDto.docs = userCouponList;
                await _couponService.UpdateBulkAsync(couponListDto);

                //if (maxWinCount >= 3)
                //{
                //    PushNotification push = PushNotificationCoupon.Build(maxWinCount, cekilis.tarih_view, user.token);
                //    await _pushService.SendPush(push);
                //}
            }
            */
        }
        
        private int GetWinCount(CouponDto coupon, Cekilis cekilis)
        {
            string[] couponNumbers = coupon.Numbers.Split('-');
            string[] cekilisNumbers = cekilis.numbers.Split('-');

            return couponNumbers.Where(n => cekilisNumbers.Contains(n)).Count();
        }

        private void CouponUpdateFinished()
        {
            UpdateCouponEventArgs args = new UpdateCouponEventArgs()
            {
                job = this
            };
            onCouponUpdateFinished.Invoke(this, args);
        }

    }
    public class UpdateCouponEventArgs : EventArgs
    {
        public IJob job;
    }
}