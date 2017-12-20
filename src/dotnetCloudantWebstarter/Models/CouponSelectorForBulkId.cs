using Newtonsoft.Json;
using System.Collections.Generic;

namespace CloudantDotNet.Models
{
    public class CouponSelectorForBulkId
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Or
        {
            public string CouponId { get; set; }
        }

        public class Selector
        {
            [JsonProperty(PropertyName = "$or")]
            public List<Or> or { get; set; }
        }

        public static CouponSelectorForBulkId Build(List<string> couponIds)
        {
            CouponSelectorForBulkId couponsSelector = new CouponSelectorForBulkId();
            couponsSelector.selector = new Selector();
            couponsSelector.selector.or = new List<Or>();
            foreach(string couponId in couponIds)
            {
                Or couponIdOr = new Or();
                couponIdOr.CouponId = couponId;
                couponsSelector.selector.or.Add(couponIdOr);
            }
            couponsSelector.fields = new List<string>();
            couponsSelector.fields.Add("_id");
            couponsSelector.fields.Add("_rev");
            couponsSelector.fields.Add("CouponId");
            return couponsSelector;
        }


        //public Selector selector { get; set; }
        //public List<string> fields { get; set; }

        //public class Selector
        //{
        //    public string CouponId { get; set; }
        //}

        //public static CouponSelectorForId Build(string couponId)
        //{
        //    CouponSelectorForId couponSelector = new CouponSelectorForId();
        //    couponSelector.selector = new Selector();
        //    couponSelector.selector.CouponId = couponId;
        //    couponSelector.fields = new List<string>();
        //    couponSelector.fields.Add("_id");
        //    couponSelector.fields.Add("_rev");
        //    return couponSelector;
        //}

    }
}
