using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Collections.Generic;
using System;
using System.Net;
using Sieve.Models;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailAnsweredController : CustomController<MailAnswered, MailAnsweredService>
    {
        public readonly MailAnsweredService service;

        public MailAnsweredController(MailAnsweredService _service, ILogger<MailAnsweredController> logger) : base(_service, logger)
        {
            this.service = _service;
        }


        [HttpGet]
        [Authorize]
        [Route("v2")]
        public override HttpResponse<List<MailAnswered>> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            if (String.IsNullOrEmpty(filterQuery.Filters)) filterQuery.Filters = "Status != 3";
            return base.GetAll_v2(filterQuery);
        }

        [HttpPut]
        [Authorize]
        [Route("bulkStatusUpdate")]
        public HttpResponse<List<MailAnswered>> BulkUpdate([FromBody] List<MailAnswered> maillList)
        {
            logger.LogInformation($"{this.GetType().Name} - Put Mail Answered List");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                maillList.ForEach(mail => {
                    mail.UpdatedBy = userAuditId;
                    mail.UpdatedAt = utfNowTime;
                });

                List<MailAnswered> newMailList = this.service.UpdateRange(maillList);

                return new HttpResponse<List<MailAnswered>>
                {
                    IsStatusCodeSuccess = true,
                    data = newMailList,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<MailAnswered>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Lista de e-mail não atualizada"
                };
            }
        }
    }
}
