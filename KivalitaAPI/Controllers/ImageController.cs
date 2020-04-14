
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using KivalitaAPI.Common;
using System;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : CustomController<Image, ImageService>
    {
        public ImageController(ImageService service, ILogger<ImageController> logger) : base(service, logger) { }
    }
}
