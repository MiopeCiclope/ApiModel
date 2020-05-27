using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.Queues;
using System.Collections.Generic;

namespace KivalitaAPI.Controllers
{
	[Route ("api/[controller]")]
	[ApiController]
	public class LeadsController : CustomController<Leads, LeadsService>
    {
        public readonly LeadsService service;
        GetEmailService _getEmailService;

        public LeadsController (GetEmailService getEmailService, LeadsService _service, ILogger<LeadsController> logger) : base (_service, logger) {
            this.service = _service;
            this._getEmailService = getEmailService;
        }

        [HttpGet]
        [Route("{linkedInID}/exists")]
        public virtual HttpResponse<Boolean> Get(string linkedInID)
        {
            logger.LogInformation($"{this.GetType().Name} - Exists");
            try
            {
                Boolean leadExists = service.LeadExists(linkedInID);
                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = leadExists
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
                    ErrorMessage = "Erro ao realizar a requisi??o"
                };
            }
        }

        [HttpPost]
        [Route("list")]
        public HttpResponse<string> Post([FromBody] List<Leads> leads)
        {
            logger.LogInformation($"{this.GetType().Name} - Post Lead List");
            try
            {
                var userAuditId = base.GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usu�rio v�lido.");
                leads.ForEach(lead => {
                    lead.CreatedBy = userAuditId;
                    lead.CreatedAt = utfNowTime;
                });

                DefaultQueue queue = new DefaultQueue();
                List<Leads> newLeads = this.service.AddRange(leads);

                foreach (Leads lead in newLeads)
                {
                    queue.Enqueue(() => _getEmailService.FromLeadAsync(lead));
                }

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = "Leads salva com sucesso!",
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
                    ErrorMessage = "Erro ao realizar a requisi��o"
                };
            }
        }

        [HttpGet]
        [Route("dates")]
        public HttpResponse<List<DateTime>> GetDates()
        {
            logger.LogInformation($"{this.GetType().Name} - Dates");
            try
            {
                var LeadsDates = this.service.GetDates();
                return new HttpResponse<List<DateTime>>
                {
                    IsStatusCodeSuccess = true,
                    data = LeadsDates,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<DateTime>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi��o"
                };
            }
        }
    }
}
