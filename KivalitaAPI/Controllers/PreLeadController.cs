
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Collections.Generic;
using System;
using System.Net;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreLeadController : CustomController<PreLead, PreLeadService>
    {
        public PreLeadController(PreLeadService service, ILogger<PreLeadController> logger) : base(service, logger) { }

        [HttpPost]
        [Authorize]
        [Route("list")]
        public HttpResponse<string> Post([FromBody] List<string> emails)
        {
            logger.LogInformation($"{this.GetType().Name} - Post Pre Lead List");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;
                var preLeads = new List<PreLead> { };

                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                emails.ForEach(email => {

                    var preLead = new PreLead
                    {
                        Email = email,
                        CreatedBy = userAuditId,
                        CreatedAt = utfNowTime
                    };

                    service.Add(preLead);
                });

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = "Pre-Leads salva com sucesso!",
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }
    }
}
