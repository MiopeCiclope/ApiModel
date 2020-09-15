
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System;
using System.Net;
using KivalitaAPI.DTOs;
using System.Collections.Generic;
using Sieve.Models;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowController : CustomController<Flow, FlowService>
    {
        public FlowController(FlowService service, ILogger<FlowController> logger) : base(service, logger) { }

        [HttpGet]
        [Authorize]
        [Route("{flowId}/report")]
        public HttpResponse<FlowReportDTO> GetReport(int flowId)
        {
            logger.LogInformation($"{this.GetType().Name} - Flow Report");
            try
            {
                FlowReportDTO report = service.getReport(flowId);
                return new HttpResponse<FlowReportDTO>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = report
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<FlowReportDTO>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{flowId}/leads")]
        public HttpResponse<List<FlowLeadsDTO>> GetLeads(int flowId, [FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - Flow Leads");
            try
            {
                var flowLeads = service.getLeads(flowId, filterQuery);
                return new HttpResponse<List<FlowLeadsDTO>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = flowLeads
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<FlowLeadsDTO>>
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
