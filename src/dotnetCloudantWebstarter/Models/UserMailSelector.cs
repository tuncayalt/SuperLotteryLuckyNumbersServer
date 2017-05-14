using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class UserMailSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string user_mail { get; set; }
        }

        public static UserMailSelector Build(string user_mail)
        {
            UserMailSelector userSelector = new UserMailSelector();
            userSelector.selector = new UserMailSelector.Selector();
            userSelector.selector.user_mail = user_mail;
            userSelector.fields = new List<string>();
            userSelector.fields.Add("_id");
            userSelector.fields.Add("_rev");
            userSelector.fields.Add("token");
            userSelector.fields.Add("user_mail");
            userSelector.fields.Add("push_win");
            userSelector.fields.Add("push_cekilis");
            userSelector.fields.Add("time");
            return userSelector;
        }
    }
}
