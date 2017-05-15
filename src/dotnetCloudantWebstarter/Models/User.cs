using System.Runtime.Serialization;

namespace CloudantDotNet.Models
{
    public class User
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string user_mail { get; set; }
        public string token { get; set; }
        public string push_cekilis { get; set; }
        public string push_win { get; set; }
        public string time { get; set; }
    }
    public class UserDbDto
    {
        public string user_mail { get; set; }
        public string token { get; set; }
        public string push_cekilis { get; set; }
        public string push_win { get; set; }
        public string time { get; set; }
    }

    public class UserRequestDto
    {
        public string prev_token { get; set; }
        public string recent_token { get; set; }
        public string user_mail { get; set; }
        public string prev_user_mail { get; set; }
        public string push_cekilis { get; set; }
        public string push_win { get; set; }
    }
}
