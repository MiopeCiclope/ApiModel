
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailSignatureController : CustomController<MailSignature, MailSignatureService>
    {
        public MailSignatureController(MailSignatureService service, ILogger<MailSignatureController> logger) : base(service, logger) { }
    }
}
