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

        [HttpPost]
        [Route("list")]
        public HttpResponse<string> Post([FromBody] List<Leads> leads)
        {
            var userAuditId = base.GetAuditTrailUser();
            var utfNowTime = DateTime.UtcNow;

            if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");
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

    }
}
