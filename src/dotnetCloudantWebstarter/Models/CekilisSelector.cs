using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{

    public class CekilisSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }
        public List<Sort> sort { get; set; }
        public int limit { get; set; }

        [DataContract]
        public class tarih
        {
            [DataMember(Name = "$gt")]
            public int gt { get; set; }
        }

        public class Selector
        {
            public tarih tarih { get; set; }
        }

        public class Sort
        {
            public string tarih { get; set; }
        }
    }
}
