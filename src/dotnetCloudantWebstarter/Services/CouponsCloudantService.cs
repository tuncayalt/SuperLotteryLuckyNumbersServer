using CloudantDotNet.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public class CouponsCloudantService : ICouponsCloudantService
    {
        private static readonly string _dbName = "coupons";
        private readonly Creds _cloudantCreds;
        private readonly UrlEncoder _urlEncoder;

        public CouponsCloudantService(Creds creds, UrlEncoder urlEncoder)
        {
            _cloudantCreds = creds;
            _urlEncoder = urlEncoder;
        }

        public async Task<dynamic> CreateAsync(Coupon item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName, item);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<Coupon>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            User = responseJson.User
                        ,
                            GameType = responseJson.GameType
                        ,
                            Numbers = responseJson.Numbers
                        ,
                            PlayTime = responseJson.PlayTime
                        ,
                            LotteryTime = responseJson.LotteryTime
                        ,
                            ToRemind = responseJson.ToRemind
                        ,
                            ServerCalled = responseJson.ServerCalled
                        ,
                            WinCount = responseJson.WinCount
                        });
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
            }
        }

        public async Task<dynamic> DeleteAsync(Coupon item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.DeleteAsync(_dbName + "/" + _urlEncoder.Encode(item.CouponId.ToString()));
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<Coupon>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            CouponId = responseJson.CouponId
                        ,
                            User = responseJson.User
                        ,
                            GameType = responseJson.GameType
                        ,
                            Numbers = responseJson.Numbers
                        ,
                            PlayTime = responseJson.PlayTime
                        ,
                            LotteryTime = responseJson.LotteryTime
                        ,
                            ToRemind = responseJson.ToRemind
                        ,
                            ServerCalled = responseJson.ServerCalled
                        ,
                            WinCount = responseJson.WinCount
                        });
                }
                string msg = "Failure to DELETE. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });
            }
        }

        public async Task<dynamic> GetAllAsync()
        {
            using (var client = CloudantClient())
            {
                var response = await client.GetAsync(_dbName + "/_all_docs?include_docs=true");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });
            }
        }

        public async Task<string> UpdateAsync(Coupon item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PutAsJsonAsync(_dbName + "/" + _urlEncoder.Encode(item.CouponId.ToString()), item);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<Coupon>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            CouponId = responseJson.CouponId
                        ,
                            User = responseJson.User
                        ,
                            GameType = responseJson.GameType
                        ,
                            Numbers = responseJson.Numbers
                        ,
                            PlayTime = responseJson.PlayTime
                        ,
                            LotteryTime = responseJson.LotteryTime
                        ,
                            ToRemind = responseJson.ToRemind
                        ,
                            ServerCalled = responseJson.ServerCalled
                        ,
                            WinCount = responseJson.WinCount
                        });
                }
                string msg = "Failure to PUT. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });
            }
        }

        public async Task PopulateTestData()
        {
            using (var client = CloudantClient())
            {
                // create and populate DB if it doesn't exist
                var response = await client.GetAsync(_dbName);
                if (!response.IsSuccessStatusCode)
                {
                    response = await client.PutAsync(_dbName, null);
                    if (response.IsSuccessStatusCode)
                    {
                        Task t1 = CreateAsync(JsonConvert.DeserializeObject<Coupon>("{ 'text': 'Sample 1' }"));
                        Task t2 = CreateAsync(JsonConvert.DeserializeObject<Coupon>("{ 'text': 'Sample 2' }"));
                        Task t3 = CreateAsync(JsonConvert.DeserializeObject<Coupon>("{ 'text': 'Sample 3' }"));
                        await Task.WhenAll(t1, t2, t3);
                    }
                    else
                    {
                        throw new Exception("Failed to create database " + _dbName + ". Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase);
                    }
                }
            }
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

        public async Task<dynamic> CreateListAsync(CouponList items)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName, items.Coupons);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<Coupon>();
                    return "Coupons inserted";
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
            }
        }

        public Task<dynamic> GetAllByUserName(string userName)
        {
            //CekilisSelector cekSelector = new CekilisSelector();
            //cekSelector.selector = new CekilisSelector.Selector();
            //cekSelector.selector.tarih = new CekilisSelector.tarih();
            //cekSelector.selector.tarih.gt = 0;
            //cekSelector.fields = new List<string>();
            //cekSelector.fields.Add("tarih");
            //cekSelector.fields.Add("tarih_view");
            //cekSelector.fields.Add("numbers");
            //cekSelector.limit = 1;
            //cekSelector.sort = new List<CekilisSelector.Sort>();
            //cekSelector.sort.Add(new CekilisSelector.Sort()
            //{
            //    tarih = "desc"
            //});
            throw new NotImplementedException();
        }

        public Task<dynamic> GetAllByTarih(string tarih)
        {
            throw new NotImplementedException();
        }
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