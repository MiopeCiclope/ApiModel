using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskNoteController : CustomController<TaskNote, TaskNoteService>
    {
        public readonly TaskNoteService service;

        public TaskNoteController(TaskNoteService _service, ILogger<TaskNoteController> logger) : base(_service, logger)
        {
            this.service = _service;
        }
    }
}
