using System;
using System.Threading.Tasks;
using CloudantDotNet.Models;
using System.Text.Encodings.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CloudantDotNet.Services
{
    public class CekilisCloudantService : ICekilisCloudantService
    {
        private static readonly string _dbName = "cekilis";
        private readonly Creds _cloudantCreds;
        private readonly IHttpClientFactory _factory;

        public CekilisCloudantService(Creds creds, IHttpClientFactory factory)
        {
            _cloudantCreds = creds;
            _factory = factory;
        }

        public async Task<Cekilis> CreateAsync(Cekilis item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsync(_dbName, new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var responseJsonString = await response.RequestMessage.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject(responseJsonString) as Cekilis;

                    return new Cekilis()
                    {
                        tarih = responseJson.tarih
                       ,
                        tarih_view = responseJson.tarih_view
                       ,
                        numbers = responseJson.numbers
                    };
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return null;
            }
        }

        public Task<dynamic> DeleteAsync(Cekilis item)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Cekilis>> GetAllAsync()
        {
            CekilisSelector cekSelector = CekilisSelector.Build(25);
            List<Cekilis> returnVal = new List<Cekilis>();

            using (var client = CloudantClient())
            {
                var response = await client.PostAsync(_dbName + "/_find", new StringContent(JsonConvert.SerializeObject(cekSelector), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string cekilisJson = await response.Content.ReadAsStringAsync();
                    JToken ob = JObject.Parse(cekilisJson);
                    JArray arr = (JArray)ob.SelectToken("docs");
                    Cekilis cekilis = null;
                    if (arr != null && arr.Count > 0)
                    {
                        foreach (var item in arr)
                        {
                            cekilis = new Cekilis()
                            {
                                tarih = (string)item["tarih"],
                                tarih_view = (string)item["tarih_view"],
                                numbers = (string)item["numbers"]
                            };
                            returnVal.Add(cekilis);
                        }
                        return returnVal;
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return returnVal;
            }
        }

        public async Task<Cekilis> GetAsync()
        {
            CekilisSelector cekSelector = CekilisSelector.Build(1);

            using (var client = CloudantClient())
            {
                var response = await client.PostAsync(_dbName + "/_find", new StringContent(JsonConvert.SerializeObject(cekSelector), Encoding.UTF8, "application/json"));
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
                return null;
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

            HttpClient client = _factory.CreateClient();
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
