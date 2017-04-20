﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class UserTokenSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string token { get; set; }
        }

        public static UserTokenSelector Build(string token)
        {
            UserTokenSelector userSelector = new UserTokenSelector();
            userSelector.selector = new UserTokenSelector.Selector();
            userSelector.selector.token = token;
            userSelector.fields = new List<string>();
            userSelector.fields.Add("_id");
            userSelector.fields.Add("_rev");
            userSelector.fields.Add("token");
            userSelector.fields.Add("user_mail");
            userSelector.fields.Add("push_win");
            userSelector.fields.Add("push_cekilis");
            return userSelector;
        }
    }
}