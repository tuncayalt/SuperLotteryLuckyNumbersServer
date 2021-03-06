﻿namespace CloudantDotNet.Models
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

    public class CouponDto
    {
        public string _id { get; set; }
        public string _rev { get; set; }
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

    public class CouponToDeleteDto
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string CouponId { get; set; }
        public bool _deleted { get; set; }
    }

    public class CouponAfterDeleteDto
    {
        public string id { get; set; }
        public string rev { get; set; }
        public bool ok { get; set; }
    }
}