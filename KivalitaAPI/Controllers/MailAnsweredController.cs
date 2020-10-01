using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailAnsweredController : CustomController<MailAnswered, MailAnsweredService>
    {
        public readonly MailAnsweredService service;

        public MailAnsweredController(MailAnsweredService _service, ILogger<MailAnsweredController> logger) : base(_service, logger)
        {
            this.service = _service;
        }
    }
}
