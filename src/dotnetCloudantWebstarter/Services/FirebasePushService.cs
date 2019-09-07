using System;
using System.Threading.Tasks;
using CloudantDotNet.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using dotnetCloudantWebstarter.Models;
using CloudantDotNet.Extensions;

namespace CloudantDotNet.Services
{
    public class FirebasePushService : IPushService
    {
        private readonly IHttpClientFactory _factory;
        string baseAddress = @"https://fcm.googleapis.com/fcm/send";
        string serverKey = "AAAAqgc7ehk:APA91bH8pJx31iSiI8JHY_sWNFO8zL-2d906p4gYNuXtAUTgD2d7juMQh3O2KkBA8yHdyu-YxtxmzXpT_yBy8elOWElVBWizHVpPM9BcQ-o99417Dss1LREP4N-qUs82YFivpDGSrI-Y";

        public FirebasePushService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> SendPush(PushNotification push)
        {
            using (var client = FirebaseClient())
            {

                var response = await client.PostAsJsonAsync("", push);

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        PushNotificationResult resultObject = JsonConvert.DeserializeObject<PushNotificationResult>(result);
                        if (resultObject != null && resultObject.results != null && resultObject.results.Count > 0 && !string.IsNullOrWhiteSpace(resultObject.results[0].error))
                        {
                            if (resultObject.results[0].error.Equals("NotRegistered"))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        private HttpClient FirebaseClient()
        {
            HttpClient client = _factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);
            return client;
        }
    }
}
