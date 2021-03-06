
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Net;
using System.Collections.Generic;
using System;
using System.Linq;
using KivalitaAPI.Enum;
using Sieve.Models;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : CustomController<Post, PostService>
    {
        public PostController(PostService service, ILogger<PostController> logger) : base(service, logger) { }

        [HttpGet]
        [AllowAnonymous]
        public override HttpResponse<List<Post>> Get([FromQuery] Post query)
        {
            logger.LogInformation($"Post - GetAll");
            try
            {
                var dataList = service.GetAll();

                if (GetAuditTrailUser() == 0)
                {
                    dataList = dataList.Where(post => post.Published == true).ToList();
                }


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
        [AllowAnonymous]
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

        [HttpGet("{slug}/{language}")]
        [AllowAnonymous]
        public HttpResponse<Post> GetByLanguage(string slug, string language)
        {
            logger.LogInformation($"Post - Get By Language - {slug} - {language}");
            try
            {
                var lang = language == "en" ? LanguageEnum.English : LanguageEnum.Portuguese;
                var posts = service.GetBySlug(slug);
                var statusRequest = (posts == null) ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                var hasError = (statusRequest == HttpStatusCode.OK);

                Post response = posts.FirstOrDefault(p => p.Language == lang);

                return new HttpResponse<Post>
                {
                    IsStatusCodeSuccess = hasError,
                    statusCode = statusRequest,
                    data = response
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

        [HttpGet]
        [AllowAnonymous]
        [Route("v2")]
        public override HttpResponse<List<Post>> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll_v2");
            try
            {
                var dataList = service.GetAll_v2(filterQuery);

                return new HttpResponse<List<Post>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList.Items,
                    Total = dataList.TotalItems
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
    }
}
