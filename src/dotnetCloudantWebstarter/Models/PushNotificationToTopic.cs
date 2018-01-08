﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class PushNotificationToTopic : PushNotification
    {
        public static PushNotificationToTopic Build(string numbers, string tarihView, string topic)
        {
            PushNotificationToTopic push = new PushNotificationToTopic();
            push.data = new Data();
            push.data.score = numbers;
            push.data.time = tarihView;
            push.notification = new Notification();
            push.notification.title = "Super Loto cekildi";
            push.notification.body = tarihView + " tarihi cekilis sonucu: " + numbers;
            push.to = "/topics/" + topic;
            return push;
        }
    }
}
