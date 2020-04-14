
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : CustomController<Post, PostService>
    {
        public PostController(PostService service, ILogger<PostController> logger) : base(service, logger) { }
    }
}
