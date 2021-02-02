using Microsoft.AspNetCore.Mvc;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KivalitaAPI.Models;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Enum;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Graph;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly MicrosoftTokenService service;
        private readonly UserService userService;
        private readonly TemplateService templateService;
        private readonly LeadsService leadService;
        private readonly LogTaskService logTaskService;
        private readonly MailAnsweredService mailAnsweredService;

        private readonly ILogger<GraphController> logger;

        public GraphController(
            MicrosoftTokenService tokenService
            , ILogger<GraphController> _logger
            , UserService _userService
            , TemplateService _templateService
            , LeadsService _leadService
            , LogTaskService _logTaskService
            , MailAnsweredService _mailAnsweredService
        ) {
            service = tokenService;
            userService = _userService;
            templateService = _templateService;
            leadService = _leadService;
            logTaskService = _logTaskService;
            logger = _logger;
            mailAnsweredService = _mailAnsweredService;
        }

        [HttpPost]
        [Authorize]
        [Route("Auth")]
        public HttpResponse<MicrosoftToken> Auth([FromBody] MicrosoftAuthDTO authObject)
        {
            logger.LogInformation($"{this.GetType().Name} - Auth Microsoft");
            try
            {
                var user = GetAuditTrailUser();
                var didLogin = service.Auth(authObject, user);
                if (didLogin == null)
                {
                    return new HttpResponse<MicrosoftToken>
                    {
                        IsStatusCodeSuccess = false,
                        statusCode = HttpStatusCode.Forbidden,
                        data = null,
                        ErrorMessage = "Erro ao validar credenciais do Outlook"
                    };
                }

                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = didLogin,
                    Total = null
                };
            }
            catch (Exception e)
            {
                logger.LogError($"{e.Message}");
                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("RefreshToken")]
        public HttpResponse<MicrosoftToken> RefreshToken()
        {
            logger.LogInformation($"{this.GetType().Name} - Refresh Token Microsoft");
            try
            {
                var user = GetAuditTrailUser();
                var didLogin = service.RefreshToken(user);
                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = didLogin,
                    Total = null
                };
            }
            catch (Exception e)
            {
                logger.LogError($"{e.Message}");
                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public virtual HttpResponse<MicrosoftToken> Delete(int id)
        {
            logger.LogInformation($"{this.GetType().Name} - Delete - {id}");
            try
            {
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                var statusRequest = HttpStatusCode.OK;
                var createdData = service.Delete(id, userAuditId);

                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<MicrosoftToken>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual int GetAuditTrailUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity.IsAuthenticated)
                return int.Parse(identity.FindFirst("Id").Value);
            else
                return 0;
        }

        [HttpPost("SendMail")]
        [Authorize]
        public HttpResponse<bool> SendMail(FlowTask task)
        {
            logger.LogInformation($"{this.GetType().Name} - SendMail - {task.Id}");
            try 
            {
                var userId = this.GetAuditTrailUser();
                var graphClient = service.GetTokenClient(userId);
                var signature = userService.GetSignature(userId);
                var template = templateService.Get((int)task.FlowAction.TemplateId);
                var lead = leadService.Get(task.LeadId);
                //lead.Email = "romulo.carvalho@kivalita.com.br";

                if(String.IsNullOrEmpty(lead.Email)) {
                    return new HttpResponse<bool>
                    {
                        IsStatusCodeSuccess = false,
                        statusCode = HttpStatusCode.InternalServerError,
                        data = false,
                        ErrorMessage = "Lead Sem e-mail"
                    };
                }

                var mail = service.BuildEmail(lead, template, task.Id, signature);

                var result = service.SendMail(graphClient, mail, userId);

                this.logTaskService.RegisterLog(LogTaskEnum.EmailSent, lead.Id, task.Id);

                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = result
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost("Webhook/RegisterAnsweredEmails")]
        [Authorize]
        public async Task<HttpResponse<IGraphServiceSubscriptionsCollectionPage>> RegisterAnsweredEmailsAsync([FromBody] RegisterAnsweredEmailsDTO dataDTO)
        {
            logger.LogInformation($"{this.GetType().Name} - RegisterAnsweredEmails");
            try
            {
                var notifications = await service.RegisterWebhookToAnsweredEmailsAsync(dataDTO.UserId);

                return new HttpResponse<IGraphServiceSubscriptionsCollectionPage>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = notifications
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<IGraphServiceSubscriptionsCollectionPage>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
            
        }

        [HttpPost("Webhook/{userId}/GetAnsweredEmails")]
        public async Task<IActionResult> GetAnsweredEmails(int userId, [FromQuery] string validationToken = null)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAnsweredEmails");

            try
            {
                if (string.IsNullOrEmpty(validationToken))
                {
                    var body = new StreamReader(Request.Body);
                    var requestBody = await body.ReadToEndAsync();
                    var collection = JsonConvert.DeserializeObject<GraphNotificationCollection>(requestBody);

                    var graphClient = service.GetTokenClient(userId);

                    foreach (var notification in collection.Value)
                    {
                        if (notification.ChangeType == "created")
                        {
                            // Obter email pelo ID 
                            var message = await graphClient.Me.Messages[notification.ResourceData.Id].Request().GetAsync();
                            var mainMessage = await graphClient.Me.Messages.Request().Filter($"conversationId eq '{message.ConversationId}'").GetAsync();

                            var regex = new Regex(@"(track\?key+)=([^\s\""]+)");
                            var match = regex.Match(mainMessage[0].Body.Content);

                            if (match.Success)
                            {
                                var key = match.Value.Split("=").Last();
                                key = Uri.UnescapeDataString(key);

                                var decryptedKey = AesCripty.DecryptString(Setting.MailTrackSecret, key);
                                int taskId = int.Parse(decryptedKey.Split("-")[0]);
                                int leadId = int.Parse(decryptedKey.Split("-")[1]);

                                mailAnsweredService.Save(message, userId, leadId, taskId);
                            }
                        }
                    }

                    return Accepted();
                }
                else
                {
                    return Content(WebUtility.HtmlEncode(validationToken));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("Webhook/{userId}/{folderName}/GetAnsweredEmailsQualified")]
        public async Task<IActionResult> GetAnsweredEmailsQualified(int userId, string folderName, [FromQuery] string validationToken = null)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAnsweredEmails");

            try
            {
                if (string.IsNullOrEmpty(validationToken))
                {
                    var body = new StreamReader(Request.Body);
                    var requestBody = await body.ReadToEndAsync();
                    var collection = JsonConvert.DeserializeObject<GraphNotificationCollection>(requestBody);

                    var graphClient = service.GetTokenClient(userId);

                    foreach (var notification in collection.Value)
                    {
                        if (notification.ChangeType == "created")
                        {
                            // Obter email pelo ID 
                            var message = await graphClient.Me.Messages[notification.ResourceData.Id].Request().GetAsync();
                            var mainMessage = await graphClient.Me.Messages.Request().Filter($"conversationId eq '{message.ConversationId}'").GetAsync();

                            var regex = new Regex(@"(track\?key+)=([^\s\""]+)");
                            var match = regex.Match(mainMessage[0].Body.Content);

                            if (match.Success)
                            {
                                var mailStatus = MailAnsweredStatusEnum.NotFound;
                                switch (folderName)
                                {
                                    case "PokeLead Bounce":
                                        mailStatus = MailAnsweredStatusEnum.NotFound;
                                        break;
                                    case "PokeLead Positivo":
                                        mailStatus = MailAnsweredStatusEnum.Positive;
                                        break;
                                    case "PokeLead Negativo":
                                        mailStatus = MailAnsweredStatusEnum.Negative;
                                        break;
                                }

                                var key = match.Value.Split("=").Last();
                                key = Uri.UnescapeDataString(key);

                                var decryptedKey = AesCripty.DecryptString(Setting.MailTrackSecret, key);
                                int taskId = int.Parse(decryptedKey.Split("-")[0]);
                                int leadId = int.Parse(decryptedKey.Split("-")[1]);

                                mailAnsweredService.SaveQualified(message, userId, leadId, taskId, mailStatus);
                            }
                        }
                    }

                    return Accepted();
                }
                else
                {
                    return Content(WebUtility.HtmlEncode(validationToken));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // Called by pipedream every 2 days 
        [HttpGet("Cron/UpdateSubscriptions")]
        public virtual async Task<IActionResult> UpdateSubscriptions()
        {
            logger.LogInformation($"{this.GetType().Name} - Cron/UpdateSubscriptions");

            var users = userService.GetAll();

            foreach (var user in users)
            {

                var graphClient = service.GetTokenClient(user.Id);

                if (graphClient != null)
                {
                    var subscriptions = await graphClient.Subscriptions
                        .Request()
                        .GetAsync();

                    foreach (var subscription in subscriptions)
                    {
                        await graphClient.Subscriptions[subscription.Id]
                            .Request()
                            .UpdateAsync(new Subscription
                            {
                                ExpirationDateTime = DateTimeOffset.UtcNow.AddMinutes(4200)
                            });
                    }
                }
            }

            return Accepted();
        }
    }
}