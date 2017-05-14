namespace CloudantDotNet.Models
{
    public abstract class PushNotification
    {
        public Data data { get; set; }
        public Notification notification { get; set; }
        public string to { get; set; }

        public class Data
        {
            public string score { get; set; }
            public string time { get; set; }
        }

        public class Notification
        {
            public string title { get; set; }
            public string body { get; set; }
        }
    }
}
