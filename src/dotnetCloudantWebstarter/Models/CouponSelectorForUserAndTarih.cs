using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class CouponSelectorForUserAndTarih
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string User { get; set; }
            public string LotteryTime { get; set; }
            public int WinCount { get; set; }
        }

        public static CouponSelectorForUserAndTarih Build(string userMail, string lotteryTime)
        {
            CouponSelectorForUserAndTarih couponSelector = new CouponSelectorForUserAndTarih();
            couponSelector.selector = new Selector();
            couponSelector.selector.User = userMail;
            couponSelector.selector.LotteryTime = lotteryTime;
            couponSelector.selector.WinCount = -1;
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
