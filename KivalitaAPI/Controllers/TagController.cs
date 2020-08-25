
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : CustomController<Tag, TagService>
    {
        public TagController(TagService service, ILogger<TagController> logger) : base(service, logger) { }
    }
}
