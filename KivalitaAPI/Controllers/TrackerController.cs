using Microsoft.AspNetCore.Mvc;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KivalitaAPI.Models;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackerController : ControllerBase
    {
        public readonly ILogger<TrackerController> _logger;
        private MailTrackService _mailTrackService;

        public TrackerController(ILogger<TrackerController> logger, MailTrackService mailTrackService) {
            this._logger = logger;
            this._mailTrackService = mailTrackService;
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
