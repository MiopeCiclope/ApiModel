using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using KivalitaAPI.Common;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : CustomController<Filter, FilterService>
    {
        public readonly FilterService service;

        public FilterController(FilterService _service, ILogger<FilterController> logger) : base(_service, logger)
        {
            this.service = _service;

        }

        [HttpGet]
        [Authorize]
        [Route("{nameOfFilter}/exists")]
        public virtual HttpResponse<Boolean> Get(string nameOfFilter)
        {
            logger.LogInformation($"{this.GetType().Name} - Exists");
            try
            {
                Boolean filterExists = service.FilterExists(nameOfFilter);
                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = filterExists
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
                    ErrorMessage = "Erro ao realizar a requisicao"
                };
            }
        }
    }
}
