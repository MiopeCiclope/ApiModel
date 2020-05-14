
using Microsoft.AspNetCore.Mvc;
using KivalitaAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public readonly GetEmailService getEmailService;

        public GetEmailController(GetEmailService getEmailService)
        {
            this.getEmailService = getEmailService;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] RequestExtractEmailDTO request)
        {
            foreach (int id in request.ids)
            {
                await getEmailService.FromLeadId(id);
            }

            return Ok();
        }


    }



}
