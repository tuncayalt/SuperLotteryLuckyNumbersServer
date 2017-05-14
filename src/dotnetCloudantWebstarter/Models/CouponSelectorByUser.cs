using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class CouponSelectorByUser
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

        public static CouponSelectorByUser Build(string userMail)
        {
            CouponSelectorByUser couponSelector = new CouponSelectorByUser();
            couponSelector.selector = new CouponSelectorByUser.Selector();
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
