using CloudantDotNet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Linq;
using dotnetCloudantWebstarter.Cache;

namespace CloudantDotNet.Services
{
    public class CouponsCloudantService : ICouponsCloudantService
    {
        private static readonly string _dbName = "coupon";
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

        public async Task<dynamic> DeleteAsync(string couponId)
        {
            CouponSelectorForId couponSelector = CouponSelectorForId.Build(couponId);
            using (var client = CloudantClient())
            {
                CouponDto item = null;
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {

                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponListDto couponList = JsonConvert.DeserializeObject<CouponListDto>(responseJson);
                    if (couponList != null && couponList.docs != null && couponList.docs.Count > 0)
                        item = couponList.docs[0];
                }
                if (item != null)
                {
                    response = await client.DeleteAsync(_dbName + "/" + _urlEncoder.Encode(item._id.ToString()) + "?rev=" + _urlEncoder.Encode(item._rev.ToString()));
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    string msg = "Failure to DELETE. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                    Console.WriteLine(msg);
                    
                }
                return false; ;
            }
        }

        public async Task<dynamic> DeleteBulkAsync(CouponListToDeleteDto items)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_bulk_docs", items);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                string msg = "Failure to PUT. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return false;
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

        public async Task<dynamic> GetByCouponIdAsync(string couponId)
        {
            CouponSelectorForId couponSelector = CouponSelectorForId.Build(couponId);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponList couponList = JsonConvert.DeserializeObject<CouponList>(responseJson);
                    return couponList.docs[0];
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new Coupon();
            }
        }


        public async Task<string> UpdateAsync(Coupon item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PutAsJsonAsync(_dbName + "/" + _urlEncoder.Encode(item.id.ToString()) + "?rev=" + _urlEncoder.Encode(item.rev.ToString()), item);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<Coupon>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            id = responseJson.id
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

        public async Task<dynamic> UpdateBulkAsync(CouponListDto items)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_bulk_docs", items);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                string msg = "Failure to PUT. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return false;
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
            if (items == null || items.docs == null || !items.docs.Any())
                return false;

            items.docs.ForEach(c => c.ServerCalled = "T");

            //Console.WriteLine("item tarih:" + items.docs[0].LotteryTime);
            //Console.WriteLine("cekilis tarih:" + CekilisCache.cekilisList[0].tarih);

            items = SetWinCounts(items);
                
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_bulk_docs", items);
                if (response.IsSuccessStatusCode)
                {
                    //var responseJson = await response.Content.ReadAsAsync<List<Coupon>>();
                    return true;
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
            }
        }

        private CouponList SetWinCounts(CouponList items)
        {
            foreach (Coupon item in items.docs)
            {
                string itemDate = item.LotteryTime.Replace("/", "");
                Cekilis cekilis = CekilisCache.cekilisList
                    .Where(c => c.tarih.Equals(itemDate))
                    .FirstOrDefault();

                if (cekilis != null)
                {
                    string[] cekilisNumbers = cekilis.numbers.Split('-');
                    string[] couponNumbers = item.Numbers.Split('-');
                    item.WinCount = couponNumbers.Where(n => cekilisNumbers.Contains(n)).Count();
                }
            }

            return items;
        }

        public async Task<dynamic> GetAllByUserName(string userName)
        {
            CouponSelectorByUser couponSelector = CouponSelectorByUser.Build(userName);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponList couponList = JsonConvert.DeserializeObject<CouponList>(responseJson);
                    return couponList.docs;  
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<Coupon>();
            }
        }

        public async Task<dynamic> GetAllByTarih(string tarih)
        {
            string lotteryTime = tarih.Substring(0, 4) + "/" + tarih.Substring(4, 2) + "/" + tarih.Substring(6, 2);
            CouponSelectorByTarih couponSelector = CouponSelectorByTarih.Build(lotteryTime);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponListDto couponList = JsonConvert.DeserializeObject<CouponListDto>(responseJson);
                    return couponList.docs;
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<CouponDto>();
            }
        }

        public async Task<dynamic> GetAllByUserNameAndTarih(string userName, string tarih)
        {
            string lotteryTime = tarih.Substring(0, 4) + "/" + tarih.Substring(4, 2) + "/" + tarih.Substring(6, 2);
            CouponSelectorForUserAndTarih couponSelector = CouponSelectorForUserAndTarih.Build(userName, lotteryTime);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponList couponList = JsonConvert.DeserializeObject<CouponList>(responseJson);
                    return couponList.docs;
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<Coupon>();
            }
        }

        public async Task<List<CouponDto>> GetWithLimitByTarih(string tarih, int updateCouponCount)
        {
            string lotteryTime = tarih.Substring(0, 4) + "/" + tarih.Substring(4, 2) + "/" + tarih.Substring(6, 2);
            CouponSelectorWithLimitByTarih couponSelector = CouponSelectorWithLimitByTarih.Build(lotteryTime, updateCouponCount);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponListDto couponList = JsonConvert.DeserializeObject<CouponListDto>(responseJson);
                    return couponList.docs;
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<CouponDto>();
            }
        }

        public async Task<dynamic> GetListByCouponIds(List<string> couponIds)
        {
            CouponSelectorForBulkId couponSelector = CouponSelectorForBulkId.Build(couponIds);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponListToDeleteDto couponList = JsonConvert.DeserializeObject<CouponListToDeleteDto>(responseJson);
                    return couponList.docs;
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<CouponListToDeleteDto>();
            }
        }

        public async Task<dynamic> GetAllByWinCountAndTarih(string tarih)
        {
            string lotteryTime = tarih.Substring(0, 4) + "/" + tarih.Substring(4, 2) + "/" + tarih.Substring(6, 2);
            CouponSelectorByWinCountAndTarih couponSelector = CouponSelectorByWinCountAndTarih.Build(lotteryTime);
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", couponSelector);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    CouponList couponList = JsonConvert.DeserializeObject<CouponList>(responseJson);
                    return couponList.docs;
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return new List<Coupon>();
            }
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