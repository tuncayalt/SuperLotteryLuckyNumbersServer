﻿using CloudantDotNet.Extensions;
using CloudantDotNet.Models;
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
        private readonly IHttpClientFactory _factory;
        private readonly UrlEncoder _urlEncoder;

        public UserCloudantService(Creds creds, IHttpClientFactory factory)
        {
            _cloudantCreds = creds;
            _factory = factory;
            _urlEncoder = UrlEncoder.Default;

        }

        public async Task<bool> CreateAsync(User userInput)
        {
            if (userInput == null)
                return false;

            UserDbDto user = new UserDbDto()
            {
                push_cekilis = userInput.push_cekilis,
                push_win = userInput.push_win,
                token = userInput.token,
                user_id = userInput.user_id,
                time = DateTime.Now.ToString()
            };
            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName, user);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return false;
            }
        }

        public Task<dynamic> DeleteAsync(User item)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAllByUserIds(List<string> userIdList)
        {
            UserTokenSelectorByBulkId userSelector = UserTokenSelectorByBulkId.Build(userIdList);

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
                                user_id = (string)item["user_id"],
                                push_cekilis = (string)item["push_cekilis"],
                                push_win = (string)item["push_win"],
                                time = (string)item["time"]
                            };
                            userList.Add(user);
                        }
                    }
                }
                else
                {
                    string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                    Console.WriteLine(msg);
                }

                return userList;
            }

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
                                user_id = (string)item["user_id"],
                                push_cekilis = (string)item["push_cekilis"],
                                push_win = (string)item["push_win"],
                                time = (string)item["time"]
                            };
                            userList.Add(user);
                        }
                    }
                }
                else
                {
                    string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                    Console.WriteLine(msg);
                }
                
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
                                user_id = (string)item["user_id"],
                                push_cekilis = (string)item["push_cekilis"],
                                push_win = (string)item["push_win"],
                                time = (string)item["time"]
                            };
                            userList.Add(user);
                        }
                    }
                }
                else
                {
                    string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                    Console.WriteLine(msg);
                }
                
                return userList;
            }
        }

        public async Task<dynamic> GetUserAsync(string userMail)
        {
            UserIdSelector userSelector = UserIdSelector.Build(userMail);

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
                                user_id = (string)item["user_id"],
                                push_cekilis = (string)item["push_cekilis"],
                                push_win = (string)item["push_win"],
                                time = (string)item["time"]
                            };
                            userList.Add(user);
                        }
                    }
                }
                else
                {
                    string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                    Console.WriteLine(msg);
                }
                
                return userList;
            }
        }

        public async Task<bool> UpdateAsync(User item)
        {
            using (var client = CloudantClient())
            {
                //var response = await client.PutAsJsonAsync(_dbName + "/" + _urlEncoder.Encode(item._id.ToString()), item);
                var response = await client.PutAsJsonAsync(_dbName + "/" + _urlEncoder.Encode(item._id.ToString()) + "?rev=" + _urlEncoder.Encode(item._rev.ToString()), item);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                string msg = "Failure to PUT. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return false;
            }
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
    }
}
