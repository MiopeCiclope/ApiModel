
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowTaskController : CustomController<FlowTask, FlowTaskService>
    {
        public FlowTaskController(FlowTaskService service, ILogger<FlowTaskController> logger) : base(service, logger) { }
    }
}
