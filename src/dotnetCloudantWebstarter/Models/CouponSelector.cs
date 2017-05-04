using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace dotnetCloudantWebstarter.Models
{
    public class CouponSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }
        public List<Sort> sort { get; set; }

        public class Selector
        {
            public string User { get; set; }
        }

        public class Sort
        {
            public string LotteryTime { get; set; }
        }

        public static CouponSelector Build(string userMail)
        {
            CouponSelector couponSelector = new CouponSelector();
            couponSelector.selector = new CouponSelector.Selector();
            couponSelector.selector.User = userMail;
            couponSelector.sort = new List<Sort>();
            Sort sort = new Sort();
            sort.LotteryTime = "desc";
            couponSelector.sort.Add(sort);
            couponSelector.fields = new List<string>();
            couponSelector.fields.Add("_id");
            couponSelector.fields.Add("_rev");
            couponSelector.fields.Add("CouponId");
            couponSelector.fields.Add("User");
            couponSelector.fields.Add("GameType");
            couponSelector.fields.Add("Numbers");
            couponSelector.fields.Add("PlayTime");
            couponSelector.fields.Add("LotteryTime");
            couponSelector.fields.Add("ToRemind");
            couponSelector.fields.Add("ServerCalled");
            couponSelector.fields.Add("WinCount");
            return couponSelector;
        }

    }
}
