
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
        private const string formLegislacao = "Formulario Legislacao";
        private const string formNewsletter = "Newsletter";
        private static readonly HttpClient client = new HttpClient();
        private static readonly string RdUrl = "https://api.rd.services/platform/conversions?api_key=byCzSPWWqGZFIxAutxGBbakVhAMxuxShdzBj";

        public WpRdStationController(WpRdStationService service, ILogger<WpRdStationController> logger) : base(service, logger) { }

        [HttpPost]
        [Route("JivoChat")]
        public async Task<HttpResponse<WpRdStation>> JivoHookAsync([FromBody] JivoPayload jivoData)
        {
            try
            {
                if (jivoData.event_name == "chat_finished")
                {
                    var rdData = ParseJivoData(jivoData.visitor);
                    var result = sendRdData(rdData);

                    var json = new WpRdStation();
                    json.FormData = rdData;

                    var createdData = base.service.Add(json);

                    var statusRequest = HttpStatusCode.Created;
                    return new HttpResponse<WpRdStation>
                    {
                        IsStatusCodeSuccess = true,
                        statusCode = statusRequest,
                        data = createdData
                    };
                }
                else return null;
            }
            catch (Exception e)
            {
                var json = new WpRdStation();
                json.FormData = e.Message + '\n' + e.StackTrace;

                return new HttpResponse<WpRdStation>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Route("WebHook")]
        public async Task<HttpResponse<WpRdStation>> WpHookAsync()
        {
            try
            {
                var body = new StreamReader(Request.Body);
                var requestBody = await body.ReadToEndAsync();
                var rdData = ParseWordPressData(requestBody);

                var result = sendRdData(rdData);

                var json = new WpRdStation();
                json.FormData = rdData;

                var createdData = base.service.Add(json);

                var statusRequest = HttpStatusCode.Created;
                return new HttpResponse<WpRdStation>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                var json = new WpRdStation();
                json.FormData = e.Message + '\n' + e.StackTrace;

                return new HttpResponse<WpRdStation>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Route("Newsletter")]
        public async Task<HttpResponse<WpRdStation>> NewsletterAsync()
        {
            try
            {
                var body = new StreamReader(Request.Body);
                var requestBody = await body.ReadToEndAsync();
                var rdData = ParseWordPressData(requestBody, formNewsletter);

                var result = sendRdData(rdData);

                var json = new WpRdStation();
                json.FormData = rdData;

                var createdData = base.service.Add(json);

                var statusRequest = HttpStatusCode.Created;
                return new HttpResponse<WpRdStation>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                var json = new WpRdStation();
                json.FormData = e.Message + '\n' + e.StackTrace;

                return new HttpResponse<WpRdStation>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        private string sendRdData(string rdData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(RdUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(rdData);
            }

            var result = "";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private string ParseWordPressData(string messyData, string identifier = formLegislacao)
        {
            var clean = messyData.Replace('+', ' ').Replace('&', ' ').Replace("%40", "@");
            var splited = clean.Split("No Label ");
            var name = splited[1].Split('=')[1];
            var empresa = splited[2].Split('=')[1];
            var cargo = splited[3].Split('=')[1];
            var email = splited[4].Split('=')[1];
            var telefone = splited[5].Split('=')[1].Replace("form_id", "");

            var RdData = new RdStationLeadDTO
            {
                event_type = "CONVERSION",
                event_family = "CDP",
                payload = new RdStationLeadPayload
                {
                    conversion_identifier = identifier,
                    email = email,
                    name = name,
                    job_title = cargo,
                    company_name = empresa
                }
            };
            return JsonSerializer.Serialize(RdData);
        }

        private string ParseJivoData(JivoVisitor jivoLead)
        {
            var RdData = new RdStationLeadDTO
            {
                event_type = "CONVERSION",
                event_family = "CDP",
                payload = new RdStationLeadPayload
                {
                    conversion_identifier = "JivoChat",
                    email = jivoLead.email,
                    name = jivoLead.name
                }
            };
            return JsonSerializer.Serialize(RdData);
        }
    }
}
    