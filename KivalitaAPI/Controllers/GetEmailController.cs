
using KivalitaAPI.Queues;

using KivalitaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace KivalitaAPI.Controllers
{

    public class RequestExtractEmailDTO
    {
        public IList<int> ids { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GetEmailController : ControllerBase
    {

        GetEmailService _getEmailService;

        public GetEmailController(GetEmailService getEmailService)
        {
            this._getEmailService = getEmailService;
        }


        [HttpPost]
        public IActionResult Post([FromBody] RequestExtractEmailDTO request)
        {
            DefaultQueue queue = new DefaultQueue();

            foreach (int leadID in request.ids)
            {
                queue.Enqueue(() => _getEmailService.FromLeadIdAsync(leadID));

            }

            return Ok();
        }

    }



}
