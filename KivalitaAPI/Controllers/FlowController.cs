
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System;
using System.Net;
using KivalitaAPI.DTOs;

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

    }
}
