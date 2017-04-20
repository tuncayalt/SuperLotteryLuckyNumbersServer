using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class PushNotificationCekilis
    {
        public Data data { get; set; }
        public Notification notification { get; set; }
        public string to { get; set; }

        public class Data
        {
            public string numbers { get; set; }
            public string tarihView { get; set; }
        }

        public class Notification
        {
            public string title { get; set; }
            public string body { get; set; }
        }

        public static PushNotificationCekilis Build(string numbers, string tarihView, string to)
        {
            PushNotificationCekilis push = new PushNotificationCekilis();
            push.data = new Data();
            push.data.numbers = numbers;
            push.data.tarihView = tarihView;
            push.notification = new Notification();
            push.notification.title = "Super Loto cekildi";
            push.notification.body = tarihView + " tarihi cekilis sonucu: " + numbers;
            push.to = to;
            return push;
        }
    }
}
