namespace CloudantDotNet.Models
{
    public class PushNotificationCoupon : PushNotification
    {
        public static PushNotificationCoupon Build(int maxViewCount, string tarihView, string to)
        {
            PushNotificationCoupon push = new PushNotificationCoupon();
            push.data = new Data();
            push.data.score = maxViewCount.ToString();
            push.data.time = tarihView;
            push.notification = new Notification();
            push.notification.title = "Super Loto'da kazandiniz!";
            push.notification.body = "Tebrikler! " + tarihView + " tarihli cekilisten " + maxViewCount + " tutturdunuz!";
            push.to = to;
            return push;
        }
    }
}
