using CloudantDotNet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public class UserCloudantService : IUserCloudantService
    {

        private static readonly string _dbName = "user";
        private readonly Creds _cloudantCreds;
        private readonly UrlEncoder _urlEncoder;

        public UserCloudantService(Creds creds, UrlEncoder urlEncoder)
        {
            _cloudantCreds = creds;
            _urlEncoder = urlEncoder;
        }

        public Task<dynamic> CreateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> DeleteAsync(User item)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetPushCekilis()
        {
            UserPushCekilisSelector userSelector = UserPushCekilisSelector.Build();

            using (var client = CloudantClient())
            {
                List<User> userList = null;
                var response = await client.PostAsJsonAsync(_dbName + "/_find", userSelector);
                if (response.IsSuccessStatusCode)
                {
                    string userJson = await response.Content.ReadAsStringAsync();
                    JToken ob = JObject.Parse(userJson);
                    JArray arr = (JArray)ob.SelectToken("docs");
                    User user = null;
                    if (arr != null && arr.Any())
                    {
                        userList = new List<User>();
                        foreach (var item in arr)
                        {
                            user = new User()
                            {
                                _id = (string)item["_id"],
                                _rev = (string)item["_rev"],
                                token = (string)item["token"],
                                user_mail = (string)item["user_mail"],
                                push_cekilis = (bool)item["push_cekilis"],
                                push_win = (bool)item["push_win"],
                            };
                            userList.Add(user);
                        }
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return userList;
            }


        }

        public async Task<dynamic> GetTokenAsync(string token)
        {
            UserTokenSelector userSelector = UserTokenSelector.Build(token);

            using (var client = CloudantClient())
            {
                List<User> userList = null;
                var response = await client.PostAsJsonAsync(_dbName + "/_find", userSelector);
                if (response.IsSuccessStatusCode)
                {
                    string userJson = await response.Content.ReadAsStringAsync();
                    JToken ob = JObject.Parse(userJson);
                    JArray arr = (JArray)ob.SelectToken("docs");
                    User user = null;
                    if (arr != null && arr.Any())
                    {
                        userList = new List<User>();
                        foreach (var item in arr)
                        {
                            user = new User()
                            {
                                _id = (string)item["_id"],
                                _rev = (string)item["_rev"],
                                token = (string)item["token"],
                                user_mail = (string)item["user_mail"],
                                push_cekilis = (bool)item["push_cekilis"],
                                push_win = (bool)item["push_win"],
                            };
                            userList.Add(user);
                        }
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return userList;
            }
        }

        public Task<dynamic> GetUserAsync(string userMail)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateAsync(User item)
        {
            using (var client = CloudantClient())
            {
                var response = await client.PutAsJsonAsync(_dbName + "/" + _urlEncoder.Encode(item._id.ToString()), item);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsAsync<User>();
                    return JsonConvert.SerializeObject(
                        new
                        {
                            _id = responseJson._id
                        ,
                            _rev = responseJson._rev
                        ,
                            user_mail = responseJson.user_mail
                        ,
                            token = responseJson.token
                        ,
                            push_cekilis = responseJson.push_cekilis
                        ,
                            push_win = responseJson.push_win
                        });
                }
                string msg = "Failure to PUT. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return JsonConvert.SerializeObject(new { msg = msg });
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
    }
}
