using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Models
{
    public class Cekilis
    {
        public string tarih_view { get; set; }
        public string tarih { get; set; }
        public string numbers { get; set; }

        public DateTime GetDateTime()
        {
            string[] format = { "yyyyMMdd" };
            DateTime date;
            if (DateTime.TryParseExact(tarih,
                                       format,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out date))
            {
                return date;
            }
            return DateTime.MinValue;
        }
    }
}
