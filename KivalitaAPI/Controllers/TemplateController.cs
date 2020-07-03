using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : CustomController<Template, TemplateService>
    {
        public readonly TemplateService service;

        public TemplateController(TemplateService _service, ILogger<TemplateController> logger) : base(_service, logger)
        {
            this.service = _service;
        }
    }
}
