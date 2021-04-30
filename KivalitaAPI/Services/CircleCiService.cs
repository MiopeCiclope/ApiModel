using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Text.Json;

namespace KivalitaAPI.Services
{
    public class CircleCiService
    {
        private readonly string apiUrl = "https://circleci.com/api/v2/project/github/Kivalita/Website/pipeline";
        private readonly string apiUrlHQ = "https://kivalita.deployhq.com/deploy/website/zxzj6gWwphzY4KNzeWSVLAJjK-yG3ibz";
        public readonly ILogger logger;

        public CircleCiService(ILogger<CircleCiService> _logger) {
            this.logger = _logger;
        }

        public void TriggerDeploy()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("Circle-Token", "fc3c33a24702f17cb6f5e1cee133b182083402f9");

            var circleCiPayload = new { branch = "baseline"};

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(circleCiPayload));
            }

            var result = "";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            logger.LogInformation($"{this.GetType().Name} - {result}");
        }

        public void TriggerDeployHQ()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrlHQ);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var hqPayload = new
            {
                payload = new
                {
                    new_ref = "latest",
                    branch = "baseline",
                    email = "romulo.carvalho@kivalita.com.br",
                    clone_url = "https://github.com/Kivalita/Website.git"
                }
            };

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(hqPayload));
            }

            var result = "";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            logger.LogInformation($"{this.GetType().Name} - {result}");
        }
    }
}
