using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudantDotNet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudantDotNet.Services
{
    public class MilliPiyangoService : IMilliPiyangoService
    {
        string baseAddress = @"http://www.millipiyango.gov.tr/sonuclar/";
        string oyunTuru = "superloto";
        string urlTarihler = @"listCekilisleriTarihleri.php?tur=";
        string urlNumaralar = @"cekilisler/";


        public async Task<dynamic> GetCekilisFromMP(DateTime dateInMP)
        {
            using (var client = MilliPiyangoClient())
            {
                var response = await client.GetAsync(urlNumaralar + oyunTuru + "/" + dateInMP.ToString("yyyyMMdd") + ".json");
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        JToken jTo = JToken.Parse(result);
                        JToken jTo2 = jTo.SelectToken("data");
                        if (jTo2 != null)
                        {
                            Cekilis cekilis = new Cekilis()
                            {
                                tarih = dateInMP.ToString("yyyyMMdd"),
                                tarih_view = (string)jTo2.SelectToken("cekilisTarihi"),
                                numbers = getNumbersFromMP((string)jTo2.SelectToken("rakamlarNumaraSirasi"))
                            };
                            return cekilis;
                        }
                        
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });

            }
        }

        private string getNumbersFromMP(string numaralar)
        {
            string[] temp1 = numaralar.Split('-');
            string result = "";
            for (int i = 0; i < temp1.Length; i++)
            {
                result += temp1[i].Trim();

                if (i < temp1.Length - 1)
                {
                    result += "-";
                }
            }
            return result;
        }

        public async Task<dynamic> GetDateFromMP()
        {
            using (var client = MilliPiyangoClient())
            {
                var response = await client.GetAsync(urlTarihler + oyunTuru);
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        JArray arr = JArray.Parse(result);
                        if (arr != null && arr.Any())
                        {
                            string tarih = (string)arr[0]["tarih"];

                            Cekilis cekilis = new Cekilis()
                            {
                                tarih = tarih
                            };
                            return cekilis.GetDateTime();
                        }
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });

            }
        }

        private HttpClient MilliPiyangoClient()
        {
            HttpClient client = HttpClientFactory.Create(new LoggingHandler());
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
