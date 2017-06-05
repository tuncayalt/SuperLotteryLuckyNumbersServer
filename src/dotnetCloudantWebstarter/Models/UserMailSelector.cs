using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class UserIdSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string user_id { get; set; }
        }

        public static UserIdSelector Build(string user_id)
        {
            UserIdSelector userSelector = new UserIdSelector();
            userSelector.selector = new UserIdSelector.Selector();
            userSelector.selector.user_id = user_id;
            userSelector.fields = new List<string>();
            userSelector.fields.Add("_id");
            userSelector.fields.Add("_rev");
            userSelector.fields.Add("token");
            userSelector.fields.Add("user_id");
            userSelector.fields.Add("push_win");
            userSelector.fields.Add("push_cekilis");
            userSelector.fields.Add("time");
            return userSelector;
        }
    }
}
