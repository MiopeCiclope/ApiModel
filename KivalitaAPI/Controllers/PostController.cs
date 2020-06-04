
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Net;
using System.Collections.Generic;
using System;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : CustomController<Post, PostService>
    {
        public PostController(PostService service, ILogger<PostController> logger) : base(service, logger) { }

        [HttpGet]
        [Authorize]
        public override HttpResponse<List<Post>> Get()
        {
            logger.LogInformation($"Post - GetAll");
            try
            {
                var dataList = service.GetAll();

                return new HttpResponse<List<Post>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Post>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public override HttpResponse<Post> Get(int id)
        {
            logger.LogInformation($"Post - Get - {id}");
            try
            {
                var data = service.Get(id);
                var statusRequest = (data == null) ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                var hasError = (statusRequest == HttpStatusCode.OK);

                return new HttpResponse<Post>
                {
                    IsStatusCodeSuccess = hasError,
                    statusCode = statusRequest,
                    data = data
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Post>
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
