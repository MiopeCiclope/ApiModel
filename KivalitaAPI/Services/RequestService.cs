using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KivalitaAPI.Services
{
    public class RequestService
    {

        async public Task<dynamic> PostFormAsync(string url, IEnumerable<KeyValuePair<string, string>> queries)
        {

            HttpContent q = new FormUrlEncodedContent(queries);

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, q))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<dynamic>(myContent);
                    }
                }
            }

        }

        async public void GetAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                    }
                }
            }

        }

    }
}
