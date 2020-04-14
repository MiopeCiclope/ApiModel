using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Common;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CustomController<TEntity, TService> : ControllerBase
        where TEntity : class, IEntity
        where TService : IService<TEntity>
    {
        public readonly TService service;
        public readonly ILogger logger;


        public CustomController(TService _service, ILogger _logger)
        {
            this.service = _service;
            this.logger = _logger;
        }

        [HttpGet]
        [Authorize]
        public virtual HttpResponse<List<TEntity>> Get()
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll");
            try
            {
                var dataList = service.GetAll();

                return new HttpResponse<List<TEntity>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = dataList
                };
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<TEntity>>
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
        public virtual HttpResponse<TEntity> Get(int id)
        {
            logger.LogInformation($"{this.GetType().Name} - Get - {id}");
            try
            {
                var data = service.Get(id);
                var statusRequest = (data == null) ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                var hasError = (statusRequest == HttpStatusCode.OK);

                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = hasError,
                    statusCode = statusRequest,
                    data = data
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public virtual HttpResponse<TEntity> Put(int id, TEntity entity)
        {
            logger.LogInformation($"{this.GetType().Name} - Put - {id}");
            try
            {
                var statusRequest = (id != entity.Id) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
                var updatedData = service.Update(entity);

                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = (statusRequest == HttpStatusCode.OK),
                    statusCode = statusRequest,
                    data = updatedData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpPost]
        [Authorize]
        public virtual HttpResponse<TEntity> Post(TEntity entity)
        {
            logger.LogInformation($"{this.GetType().Name} - Post - {entity.ToString()}");
            try
            {
                var statusRequest = HttpStatusCode.Created;
                var createdData = service.Add(entity);

                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public virtual HttpResponse<TEntity> Delete(int id)
        {
            logger.LogInformation($"{this.GetType().Name} - Delete - {id}");
            try
            {
                var statusRequest = HttpStatusCode.OK;
                var createdData = service.Delete(id);

                return new HttpResponse<TEntity>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<TEntity>
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
