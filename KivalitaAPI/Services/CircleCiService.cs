using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Text.Json;

namespace KivalitaAPI.Services
{
    public class CircleCiService
    {
        private readonly string apiUrl = "https://circleci.com/api/v2/project/github/Kivalita/Website/pipeline";
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
    }
}
