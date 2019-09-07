using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudantDotNet.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string requestUri, object contentObject)
        {
            return await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(contentObject), Encoding.UTF8, "application/json"));
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, string requestUri, object contentObject)
        {
            return await client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(contentObject), Encoding.UTF8, "application/json"));
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var responseJsonString = await content.ReadAsStringAsync();
            return (T)JsonConvert.DeserializeObject(responseJsonString);
        }
    }
}
