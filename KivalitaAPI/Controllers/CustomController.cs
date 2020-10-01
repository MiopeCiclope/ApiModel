using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Common;
using KivalitaAPI.Interfaces;
using System;
using System.Security.Claims;
using Sieve.Models;
using System.Linq;

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
        private readonly string[] hasOwnerProperty = { "Company", "FlowTask" };

        public CustomController(TService _service, ILogger _logger)
        {
            this.service = _service;
            this.logger = _logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public virtual int GetAuditTrailUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity.IsAuthenticated)
                return int.Parse(identity.FindFirst("Id").Value);
            else
                return 0;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public virtual bool isColaborador()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.FindFirst("Role").Value == "Colaborador";
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public virtual bool isMarketing()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.FindFirst("Role").Value == "Marketing";
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public virtual bool hasOwner()
        {
            return this.hasOwnerProperty.Contains(typeof(TEntity).Name);
        }

        [HttpGet]
        [Authorize]
        public virtual HttpResponse<List<TEntity>> Get([FromQuery] TEntity query)
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
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                entity.UpdatedBy = userAuditId;
                entity.UpdatedAt = DateTime.UtcNow;

                var updatedData = service.Update(entity);

                var statusRequest = (id != entity.Id) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
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
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                entity.CreatedBy = userAuditId;
                entity.CreatedAt = DateTime.UtcNow;

                var createdData = service.Add(entity);

                var statusRequest = HttpStatusCode.Created;
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
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                var statusRequest = HttpStatusCode.OK;
                var createdData = service.Delete(id, userAuditId);

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
                    ErrorMessage = String.IsNullOrEmpty(e.Message) ? "Erro ao realizar a requisição" : e.Message
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("v2")]
        public virtual HttpResponse<List<TEntity>> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll_v2");
            try
            {
                if(hasOwner() && isColaborador()) filterQuery.Filters = $"{filterQuery.Filters},Owner=={GetAuditTrailUser()}";
                if(hasOwner() && isMarketing() && typeof(TEntity).Name != "Company") filterQuery.Filters = $"{filterQuery.Filters},Owner=={GetAuditTrailUser()}";

                var dataList = service.GetAll_v2(filterQuery);

                return new HttpResponse<List<TEntity>>
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
                return new HttpResponse<List<TEntity>>
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
