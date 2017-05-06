using System.Collections.Generic;

namespace dotnetCloudantWebstarter.Models
{
    public class CouponSelectorForId
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
            public string CouponId { get; set; }
        }

        public static CouponSelectorForId Build(string couponId)
        {
            CouponSelectorForId couponSelector = new CouponSelectorForId();
            couponSelector.selector = new Selector();
            couponSelector.selector.CouponId = couponId;
            couponSelector.fields = new List<string>();
            couponSelector.fields.Add("_id");
            couponSelector.fields.Add("_rev");
            return couponSelector;
        }

    }
}
