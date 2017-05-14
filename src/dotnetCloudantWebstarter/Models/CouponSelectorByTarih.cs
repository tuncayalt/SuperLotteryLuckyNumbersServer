﻿using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class CouponSelectorByTarih
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string LotteryTime { get; set; }
            public int WinCount { get; set; }
        }

        public static CouponSelectorByTarih Build(string tarih)
        {
            CouponSelectorByTarih couponSelector = new CouponSelectorByTarih();
            couponSelector.selector = new Selector();
            couponSelector.selector.LotteryTime = tarih;
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
