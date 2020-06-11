
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using KivalitaAPI.Common;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : CustomController<Image, ImageService>
    {
        public ImageController(ImageService service, ILogger<ImageController> logger) : base(service, logger) { }


        [HttpGet]
        [AllowAnonymous]
        public override HttpResponse<List<Image>> Get([FromQuery] Image query)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll");
            try
            {
                var dataList = new List<Image> { };

                if (!String.IsNullOrEmpty(query.Type))
                {
                    dataList = service.GetByType(query.Type);
                }
                else
                {
                    dataList = service.GetAll();
                }

                return new HttpResponse<List<Image>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Image>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

    }
}
