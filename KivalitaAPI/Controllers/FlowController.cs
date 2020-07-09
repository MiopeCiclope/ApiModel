
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowController : CustomController<Flow, FlowService>
    {
        public FlowController(FlowService service, ILogger<FlowController> logger) : base(service, logger) { }
    }
}
