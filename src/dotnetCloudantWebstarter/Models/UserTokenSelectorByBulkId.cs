using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class UserTokenSelectorByBulkId
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Or
        {
            public string user_id { get; set; }
        }

        public class Selector
        {
            [JsonProperty(PropertyName = "$or")]
            public List<Or> or { get; set; }
        }

        public static UserTokenSelectorByBulkId Build(List<string> userIdList)
        {
            UserTokenSelectorByBulkId userSelector = new UserTokenSelectorByBulkId();
            userSelector.selector = new Selector();
            userSelector.selector.or = new List<Or>();
            foreach (var item in userIdList)
            {
                Or or = new Or
                {
                    user_id = item
                };
                userSelector.selector.or.Add(or);
            }
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
