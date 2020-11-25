
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using KivalitaAPI.Common;
using System.Collections.Generic;
using System.Net;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Sieve.Models;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : CustomController<Job, JobService>
    {
        public JobController(JobService service, ILogger<JobController> logger) : base(service, logger) { }

        [HttpGet]
        [AllowAnonymous]
        public override HttpResponse<List<Job>> Get([FromQuery] Job query)
        {
            logger.LogInformation($"Job - GetAll");
            try
            {
                var dataList = service.GetAll();

                if (GetAuditTrailUser() == 0)
                {
                    dataList = dataList.Where(job => job.Published == true).ToList();
                }

                return new HttpResponse<List<Job>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Job>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public override HttpResponse<Job> Get(int id)
        {
            logger.LogInformation($"Job - Get - {id}");
            try
            {
                var data = service.Get(id);
                var statusRequest = (data == null) ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                var hasError = (statusRequest == HttpStatusCode.OK);

                return new HttpResponse<Job>
                {
                    IsStatusCodeSuccess = hasError,
                    statusCode = statusRequest,
                    data = data
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Job>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("v2")]
        public override HttpResponse<List<Job>> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll_v2");
            try
            {
                var dataList = service.GetAll_v2(filterQuery);

                if (GetAuditTrailUser() == 0)
                {
                    dataList.Items = dataList.Items.Where(job => job.Published == true).ToList();
                }

                return new HttpResponse<List<Job>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList.Items,
                    Total = dataList.TotalItems
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Job>>
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
