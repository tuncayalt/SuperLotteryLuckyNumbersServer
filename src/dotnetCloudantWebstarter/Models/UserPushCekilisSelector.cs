using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class UserPushCekilisSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string push_win { get; set; }
        }

        public static UserPushCekilisSelector Build()
        {
            UserPushCekilisSelector userSelector = new UserPushCekilisSelector();
            userSelector.selector = new Selector();
            userSelector.selector.push_win = "T";
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
