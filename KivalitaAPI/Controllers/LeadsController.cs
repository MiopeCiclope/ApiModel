using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using KivalitaAPI.Common;
using System.Net;

namespace KivalitaAPI.Controllers
{
	[Route ("api/[controller]")]
	[ApiController]
	public class LeadsController : CustomController<Leads, LeadsService>
    {
        public readonly LeadsService service;

        public LeadsController (LeadsService _service, ILogger<LeadsController> logger) : base (_service, logger) {
            this.service = _service;
        }

        [HttpPost]
        [Route("list")]
        public HttpResponse<string> Post([FromBody] Leads[] leads)
        {
            this.service.AddRange(leads);

            return new HttpResponse<string>
            {
                IsStatusCodeSuccess = true,
                data = "Leads salva com sucesso!",
                statusCode = HttpStatusCode.OK
            };
        }

    }
}
