using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public static CekilisSelector Build(int limit)
        {
            CekilisSelector cekSelector = new CekilisSelector();
            cekSelector.selector = new CekilisSelector.Selector();
            cekSelector.selector.tarih = new CekilisSelector.tarih();
            cekSelector.selector.tarih.gt = 0;
            cekSelector.fields = new List<string>();
            cekSelector.fields.Add("tarih");
            cekSelector.fields.Add("tarih_view");
            cekSelector.fields.Add("numbers");
            cekSelector.limit = limit;
            cekSelector.sort = new List<CekilisSelector.Sort>();
            cekSelector.sort.Add(new CekilisSelector.Sort()
            {
                tarih = "desc"
            });
            return cekSelector;
        }
    }
}
