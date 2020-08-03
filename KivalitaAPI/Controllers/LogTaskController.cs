
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogTaskController : CustomController<LogTask, LogTaskService>
    {
        public LogTaskController(LogTaskService service, ILogger<LogTaskController> logger) : base(service, logger) { }
    }
}
