using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class Coupon
    {
        public string id { get; set; }
        public string rev { get; set; }
        public string CouponId { get; set; }
        public string User { get; set; }
        public string GameType { get; set; }
        public string Numbers { get; set; }
        public string PlayTime { get; set; }
        public string LotteryTime { get; set; }
        public string ToRemind { get; set; }
        public string ServerCalled { get; set; }
        public int WinCount { get; set; }
    }

    //public class CouponDto
    //{
    //    public string _id { get; set; }
    //    public string id { get; set; }
    //    public string User { get; set; }
    //    public string GameType { get; set; }
    //    public string Numbers { get; set; }
    //    public string PlayTime { get; set; }
    //    public string LotteryTime { get; set; }
    //    public string ToRemind { get; set; }
    //    public string ServerCalled { get; set; }
    //    public int WinCount { get; set; }
    //}
}
