using Microsoft.AspNetCore.Mvc;
using System;
using KivalitaAPI.Common;
using KivalitaAPI.Services;
using KivalitaAPI.Models;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Enum;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackerController : ControllerBase
    {
        public readonly ILogger<TrackerController> _logger;
        private MailTrackService _mailTrackService;
        private LogTaskService _logTaskService;

        public TrackerController(
            ILogger<TrackerController> logger,
            MailTrackService mailTrackService,
            LogTaskService logTaskService
        ) {
            this._logger = logger;
            this._mailTrackService = mailTrackService;
            this._logTaskService = logTaskService;
        }

        [HttpGet]
        [Route("Track")]
        public IActionResult Track([FromQuery] string key)
        {
            try
            {
                _logger.LogInformation($"{this.GetType().Name} - Track E-mail");

                var decryptedKey = AesCripty.DecryptString(Setting.MailTrackSecret, key);
                int taskId = int.Parse(decryptedKey.Split("-")[0]);
                int leadId = int.Parse(decryptedKey.Split("-")[1]);
                this._mailTrackService.Add(new MailTrack { TaskId = taskId, LeadId = leadId, CreatedAt = DateTime.UtcNow});
                this._logTaskService.RegisterLog(LogTaskEnum.EmailRead, leadId, taskId);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
            byte[] imgbytes = Convert.FromBase64String("R0lGODlhAQABAIAAANvf7wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");
            return File(imgbytes, "image/jpeg");
        }
    }
}
