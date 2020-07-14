using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        public readonly ILogger<SchedulerController> logger;
        public readonly IJobScheduler _scheduler;

        public SchedulerController(ILogger<SchedulerController> logger, IJobScheduler scheduler)
        {
            this.logger = logger;
            this._scheduler = scheduler;
        }

        [HttpPost]
        [Authorize]
        public async Task<HttpResponse<string>> Post([FromBody] JobScheduleDTO job, CancellationToken cancellationToken)
        {
            logger.LogInformation($"{this.GetType().Name} - Schedule Job - {job.JobType.Name}");
            try
            {
                job.userId = this.GetAuditTrailUser();
                var datetimeJob = await _scheduler.ScheduleJob(cancellationToken, job);
                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.Created,
                    data = datetimeJob.ToString(),
                    ErrorMessage = null
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

        [HttpGet]
        [Authorize]
        public HttpResponse<List<JobScheduleDTO>> Get()
        {
            logger.LogInformation($"{this.GetType().Name} - Schedule Job - Get All Jobs");
            try
            {
                var jobs = _scheduler.GetScheduledJobs();
                return new HttpResponse<List<JobScheduleDTO>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.Created,
                    data = jobs,
                    ErrorMessage = null
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<JobScheduleDTO>>
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
