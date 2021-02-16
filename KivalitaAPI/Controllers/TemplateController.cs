using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using System.Net;
using KivalitaAPI.Common;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : CustomController<Template, TemplateService>
    {
        public readonly TemplateService service;
        public readonly TemplateTransformService templateTransformService;
        public readonly LeadsService leadsService;

        public TemplateController(
            TemplateService _service,
            TemplateTransformService _templateTransformService,
            LeadsService _leadsService,
            ILogger<TemplateController> logger) : base(_service, logger)
        {
            this.service = _service;
            this.templateTransformService = _templateTransformService;
            this.leadsService = _leadsService;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}/transform")]
        public HttpResponse<string> Transform(int id, [FromQuery] int leadId)
        {
            logger.LogInformation($"{this.GetType().Name} - Transform template");
            try
            {
                var lead = leadsService.Get(leadId);
                var template = service.Get(id);

                var templateRendered = templateTransformService.TransformLead(template.Content, lead);

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = templateRendered,
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
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("TransformByData")]
        public virtual HttpResponse<string> TransformByData(Template template)
        {
            logger.LogInformation($"{this.GetType().Name} - TransformByData - {template.ToString()}");
            try
            {
                var lead = leadsService.GetAll_v2(null).Items.First();

                var templateRendered = templateTransformService.Transform(template.Content, lead);

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = templateRendered,
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
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }
    }
}
