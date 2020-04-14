
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : CustomController<Job, JobService>
    {
        public JobController(JobService service, ILogger<JobController> logger) : base(service, logger) { }
    }
}
