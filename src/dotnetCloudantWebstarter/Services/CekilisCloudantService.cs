using System;
using System.Threading.Tasks;
using CloudantDotNet.Models;
using System.Text.Encodings.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CloudantDotNet.Services
{
    public class CekilisCloudantService : ICekilisCloudantService
    {
        private static readonly string _dbName = "cekilis";
        private readonly Creds _cloudantCreds;
        private readonly UrlEncoder _urlEncoder;

        public CekilisCloudantService(Creds creds, UrlEncoder urlEncoder)
        {
            _cloudantCreds = creds;
            _urlEncoder = urlEncoder;
        }

        public async Task<dynamic> CreateAsync(Cekilis item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName, item);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.RequestMessage.Content.ReadAsAsync<Cekilis>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            tarih = responseJson.tarih
                        ,
                            tarih_view = responseJson.tarih_view
                        ,
                            numbers = responseJson.numbers
                        });
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
            }
        }

        public Task<dynamic> DeleteAsync(Cekilis item)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetAsync()
        {
            CekilisSelector cekSelector = new CekilisSelector();
            cekSelector.selector = new CekilisSelector.Selector();
            cekSelector.selector.tarih = new CekilisSelector.tarih();
            cekSelector.selector.tarih.gt = 0;
            cekSelector.fields = new List<string>();
            cekSelector.fields.Add("tarih");
            cekSelector.fields.Add("tarih_view");
            cekSelector.fields.Add("numbers");
            cekSelector.limit = 1;
            cekSelector.sort = new List<CekilisSelector.Sort>();
            cekSelector.sort.Add(new CekilisSelector.Sort()
            {
                tarih = "desc"
            });

            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", cekSelector);
                if (response.IsSuccessStatusCode)
                {
                    string cekilisJson = await response.Content.ReadAsStringAsync();
                    JToken ob = JObject.Parse(cekilisJson);
                    JArray arr = (JArray)ob.SelectToken("docs");
                    Cekilis cekilis = null;
                    if (arr != null && arr.Count > 0)
                    {
                        cekilis = new Cekilis()
                        {
                            tarih = (string)arr[0]["tarih"],
                            tarih_view = (string)arr[0]["tarih_view"],
                            numbers = (string)arr[0]["numbers"]
                        };
                        return cekilis;
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });
            }
        }

        public Task PopulateTestData()
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAsync(Coupon item)
        {
            throw new NotImplementedException();
        }

        private HttpClient CloudantClient()
        {
            if (_cloudantCreds.username == null || _cloudantCreds.password == null || _cloudantCreds.host == null)
            {
                throw new Exception("Missing Cloudant NoSQL DB service credentials");
            }

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_cloudantCreds.username + ":" + _cloudantCreds.password));

            HttpClient client = HttpClientFactory.Create(new LoggingHandler());
            client.BaseAddress = new Uri("https://" + _cloudantCreds.host);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
            return client;
        }
        class LoggingHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                Console.WriteLine("{0}\t{1}", request.Method, request.RequestUri);
                var response = await base.SendAsync(request, cancellationToken);
                Console.WriteLine(response.StatusCode);
                return response;
            }
        }
    }
}
