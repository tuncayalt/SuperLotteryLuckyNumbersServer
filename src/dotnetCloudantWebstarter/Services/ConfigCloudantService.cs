using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CloudantDotNet.Models;
using dotnetCloudantWebstarter.Models;
using Newtonsoft.Json.Linq;

namespace dotnetCloudantWebstarter.Services
{
    public class ConfigCloudantService : IConfigCloudantService
    {
        private static readonly string _dbName = "config";
        private readonly Creds _cloudantCreds;
        private readonly UrlEncoder _urlEncoder;

        public ConfigCloudantService(Creds creds, UrlEncoder urlEncoder)
        {
            _cloudantCreds = creds;
            _urlEncoder = urlEncoder;
        }

        public async Task<Config> GetAsync()
        {
            ConfigSelector configSelector = ConfigSelector.Build();

            using (var client = CloudantClient())
            {
                var response = await client.PostAsJsonAsync(_dbName + "/_find", configSelector);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JToken ob = JObject.Parse(result);
                    JArray arr = (JArray)ob.SelectToken("docs");
                    Config config = null;
                    if (arr != null && arr.Count > 0)
                    {
                        config = new Config()
                        {
                            environment = (string)arr[0]["environment"],
                            superLotoTopic = (string)arr[0]["superLotoTopic"]
                        };
                        return config;
                    }
                }
                string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
                Console.WriteLine(msg);
                return null;
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
