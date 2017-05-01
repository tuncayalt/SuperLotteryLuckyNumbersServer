using CloudantDotNet.Models;
using CloudantDotNet.Services;
using System;
using System.Collections.Generic;

namespace CloudantDotNet.Tasks
{
    public class CekilisPushJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday};
        public int startHour { get; set; } = 18;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(3);
        public DateTime lastWorked { get; set; }

        ICekilisCloudantService _cekilisService;
        IUserCloudantService _userService;
        IPushService _pushService;

        public event EventHandler<PushCekilisEventArgs> onCekilisPushFinished;

        public CekilisPushJob(ICekilisCloudantService cekilisService, IUserCloudantService userService, IPushService pushService)
        {
            _cekilisService = cekilisService;
            _userService = userService;
            _pushService = pushService;
        }

        public void StartJob()
        {
            SendPushToUsers();

            Console.WriteLine("CekilisPushJob ran:" + DateTime.UtcNow.GetTurkeyTime());
        }

        private async void SendPushToUsers()
        {
            Cekilis cekilis = await _cekilisService.GetAsync();
            if (cekilis == null)
                return;

            List<User> userList = await _userService.GetPushCekilis();

            if (userList != null)
            {
                List<PushNotificationCekilis> pushList = new List<PushNotificationCekilis>();
                foreach (var user in userList)
                {
                    PushNotificationCekilis push = PushNotificationCekilis.Build(cekilis.numbers, cekilis.tarih_view, user.token);
                    await _pushService.SendPush(push);
                }
            }

            PushFinished();
        }

        private void PushFinished()
        {
            PushCekilisEventArgs args = new PushCekilisEventArgs()
            {
                job = this
            };
            onCekilisPushFinished.Invoke(this, args);
        }
    }
    public class PushCekilisEventArgs : EventArgs
    {
        public IJob job;
    }


}