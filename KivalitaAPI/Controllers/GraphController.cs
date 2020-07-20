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
        private readonly ILogger<GraphController> logger;

        public GraphController(MicrosoftTokenService tokenService, ILogger<GraphController> _logger) {
            service = tokenService;
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
    }
}
