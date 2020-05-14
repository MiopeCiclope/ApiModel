
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using KivalitaAPI.Common;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using KivalitaAPI.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WpRdStationController : CustomController<WpRdStation, WpRdStationService>
    {
        private static readonly HttpClient client = new HttpClient();
        public WpRdStationController(WpRdStationService service, ILogger<WpRdStationController> logger) : base(service, logger) { }

        [HttpPost]
        [Route("JivoChat")]
        public async Task<HttpResponse<WpRdStation>> JivoHookAsync([FromBody] JivoPayload jivoData)
        {
            try
            {
                var teste = new WpRdStation();
                teste.FormData = JsonSerializer.Serialize(jivoData);
                return base.Post(teste);
            }
            catch (Exception e)
            {
                var teste = new WpRdStation();
                teste.FormData = e.Message + '\n' + e.StackTrace;
                return base.Post(teste);
            }
        }

        [HttpPost]
        [Route("WebHook")]
        public async Task<HttpResponse<WpRdStation>> WpHookAsync()
        {
            try
            {
                var qwer = new StreamReader(Request.Body);
                var requestBody = await qwer.ReadToEndAsync();
                var clean = requestBody.Replace('+', ' ').Replace('&', ' ').Replace("%40", "@");
                var splited = clean.Split("No Label ");
                var name = splited[1].Split('=')[1];
                var empresa = splited[2].Split('=')[1];
                var cargo = splited[3].Split('=')[1];
                var email = splited[4].Split('=')[1];
                var telefone = splited[5].Split('=')[1].Replace("form_id", "");

                string url = "https://api.rd.services/platform/conversions?api_key=byCzSPWWqGZFIxAutxGBbakVhAMxuxShdzBj";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = "{\"event_type\":\"CONVERSION\"," +
                            "\"event_family\":\"CDP\"," +
                            "\"payload\":{" +
                            "\"conversion_identifier\":\"Formulario Legislacao\"," +
                            "\"email\":\"" + email + "\"," +
                            "\"name\":\"" + name + "\"," +
                            "\"job_title\":\"" + cargo + "\"," +
                            "\"company_name\":\"" + empresa + "\"}}";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
                var teste = new WpRdStation();
                teste.FormData = json;
                return base.Post(teste);
            }
            catch(Exception e)
            {
                var teste = new WpRdStation();
                teste.FormData = e.Message+'\n'+e.StackTrace;
                return base.Post(teste);
            }
        }

        //private string ParseWordPressData(string messyData)
        //{

        //}
    }
}
    