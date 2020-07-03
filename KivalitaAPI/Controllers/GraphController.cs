using Microsoft.AspNetCore.Mvc;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly MicrosoftTokenService service;

        public GraphController(MicrosoftTokenService tokenService) {
            service = tokenService;
        }

        [HttpPost]
        [Authorize]
        [Route("Auth")]
        public HttpResponse<bool> Auth([FromBody] MicrosoftAuthDTO authObject)
        {
            try
            {
                var user = GetAuditTrailUser();
                var didLogin = service.Auth(authObject, user);
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = didLogin,
                    Total = null
                };
            }
            catch (Exception e)
            {
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("RefreshToken")]
        public HttpResponse<bool> RefreshToken()
        {
            try
            {
                var user = GetAuditTrailUser();
                var didLogin = service.RefreshToken(user);
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = didLogin,
                    Total = null
                };
            }
            catch (Exception e)
            {
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
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
