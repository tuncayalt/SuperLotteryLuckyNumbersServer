using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class Coupon
    {
        public long CouponId { get; set; }
        public string User { get; set; }
        public string GameType { get; set; }
        public string Numbers { get; set; }
        public DateTime PlayTime { get; set; }
        public DateTime LotteryTime { get; set; }
        public string ToRemind { get; set; }
        public string ServerCalled { get; set; }
        public int WinCount { get; set; }
    }
}
