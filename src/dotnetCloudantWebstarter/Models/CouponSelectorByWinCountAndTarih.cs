using Newtonsoft.Json;
using System.Collections.Generic;


namespace CloudantDotNet.Models
{
    public class CouponSelectorByWinCountAndTarih
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class WinCount
        {
            [JsonProperty(PropertyName = "$gte")]
            public int gte { get; set; }
        }

        public class Selector
        {
            public string LotteryTime { get; set; }
            public WinCount WinCount { get; set; }
        }

        public static CouponSelectorByWinCountAndTarih Build(string lotteryTime)
        {
            CouponSelectorByWinCountAndTarih couponSelector = new CouponSelectorByWinCountAndTarih();
            couponSelector.selector = new Selector();
            couponSelector.selector.LotteryTime = lotteryTime;
            couponSelector.selector.WinCount = new WinCount();
            couponSelector.selector.WinCount.gte = 3;
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
