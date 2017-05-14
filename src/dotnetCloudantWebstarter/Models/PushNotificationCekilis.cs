namespace CloudantDotNet.Models
{
    public class PushNotificationCekilis : PushNotification
    {
        public static PushNotificationCekilis Build(string numbers, string tarihView, string to)
        {
            PushNotificationCekilis push = new PushNotificationCekilis();
            push.data = new Data();
            push.data.score = numbers;
            push.data.time = tarihView;
            push.notification = new Notification();
            push.notification.title = "Super Loto cekildi";
            push.notification.body = tarihView + " tarihi cekilis sonucu: " + numbers;
            push.to = to;
            return push;
        }
    }
}
