using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class User
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string user_mail { get; set; }
        public string token { get; set; }
        public bool push_cekilis { get; set; }
        public bool push_win { get; set; }
    }
}
