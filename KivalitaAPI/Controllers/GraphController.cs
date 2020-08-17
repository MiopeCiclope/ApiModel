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

        private readonly ILogger<GraphController> logger;

        public GraphController(MicrosoftTokenService tokenService
            , ILogger<GraphController> _logger
            , UserService _userService
            , TemplateService _templateService
            , LeadsService _leadService
        ) {
            service = tokenService;
            userService = _userService;
            templateService = _templateService;
            leadService = _leadService;
            logger = _logger;
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
        public virtual HttpResponse<bool> SendMail(FlowTask task)
        {
            logger.LogInformation($"{this.GetType().Name} - SendMail - {task.Id}");
            try
            {
                var userId = this.GetAuditTrailUser();
                var graphClient = service.GetTokenClient(userId);
                var signature = userService.GetSignature(userId);
                var template = templateService.Get((int)task.FlowAction.TemplateId);
                var lead = leadService.Get(task.LeadId);

                var mail = service.BuildEmail(lead, template, task.Id, signature);

                var result = service.SendMail(graphClient, mail, userId);

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
    }
}
