
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Collections.Generic;
using Sieve.Models;
using System.Net;
using System;
using KivalitaAPI.DTOs;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowTaskController : CustomController<FlowTask, FlowTaskService>
    {
        public FlowTaskController(FlowTaskService service, ILogger<FlowTaskController> logger) : base(service, logger) { }

        [HttpGet]
        [Authorize]
        [Route("v2/initialList")]
        public virtual HttpResponse<TaskListDTO> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll_v2");
            try
            {
                if (hasOwner() && isColaborador()) filterQuery.Filters = $"{filterQuery.Filters},Owner=={GetAuditTrailUser()}";
                if (hasOwner() && isMarketing() && typeof(FlowTask).Name != "Company") filterQuery.Filters = $"{filterQuery.Filters},Owner=={GetAuditTrailUser()}";

                var dataList = service.initialTasks(filterQuery);

                return new HttpResponse<TaskListDTO>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList,
                    Total = 0
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<TaskListDTO>
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
