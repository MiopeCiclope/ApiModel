using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.Queues;
using System.Collections.Generic;
using KivalitaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        public readonly LeadsService service;
        public readonly CompanyService companyService;
        public readonly GetEmailService _getEmailService;
        public readonly ILogger<LeadsController> logger;

        public LeadsController(CompanyService companyService, GetEmailService getEmailService, LeadsService _service, ILogger<LeadsController> logger)
        {
            this.service = _service;
            this.companyService = companyService;
            this._getEmailService = getEmailService;
            this.logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual int GetAuditTrailUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
                return int.Parse(identity.FindFirst("Id").Value);
            else
                return 0;
        }

        [HttpGet]
        [Authorize]
        public HttpResponse<List<Leads>> Get([FromQuery] LeadQueryDTO leadQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll");
            try
            {
                var queryResult = service.FetchAll(leadQuery);

                return new HttpResponse<List<Leads>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = queryResult.Items,
                    Total = queryResult.TotalItems
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Leads>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{linkedInID}/exists")]
        public virtual HttpResponse<Boolean> Get(string linkedInID)
        {
            logger.LogInformation($"{this.GetType().Name} - Exists");
            try
            {
                Boolean leadExists = service.LeadExists(linkedInID);
                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = leadExists
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("availableCompanies")]
        public virtual HttpResponse<List<Company>> GetCompanies()
        {
            logger.LogInformation($"{this.GetType().Name} - GetCompanies");
            try
            {
                List<Company> AvailableCompanies = companyService.WithOutOwner();
                return new HttpResponse<List<Company>>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = HttpStatusCode.OK,
                    data = AvailableCompanies
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<Company>>
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
        [Route("list")]
        public HttpResponse<string> Post([FromBody] List<LeadDTO> leads)
        {
            logger.LogInformation($"{this.GetType().Name} - Post Lead List");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usuário válido.");

                leads.ForEach(lead => {
                    lead.CreatedBy = userAuditId;
                    lead.CreatedAt = utfNowTime;
                });

                DefaultQueue queue = new DefaultQueue();
                List<Leads> newLeads = this.service.SaveRange(leads);

                foreach (Leads lead in newLeads)
                {
                    queue.Enqueue(() => _getEmailService.FromLeadAsync(lead));
                }

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = "Leads salva com sucesso!",
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("dailyLeads")]
        public HttpResponse<int> GetDailyLeads()
        {
            logger.LogInformation($"{this.GetType().Name} - Daily Leads");
            try
            {
                var totalLeads = this.service.GetDailyLeads();
                return new HttpResponse<int>
                {
                    IsStatusCodeSuccess = true,
                    data = totalLeads,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<int>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = 0,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("dates")]
        public HttpResponse<List<GroupOwnerLeadDTO>> GetDates()
        {
            logger.LogInformation($"{this.GetType().Name} - Dates");
            try
            {
                var LeadsDates = this.service.GetDates();
                return new HttpResponse<List<GroupOwnerLeadDTO>>
                {
                    IsStatusCodeSuccess = true,
                    data = LeadsDates,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<GroupOwnerLeadDTO>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisição"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("filter")]
        public HttpResponse<LeadFilterDTO> GetFilter([FromQuery] LeadQueryDTO leadQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - Filter");
            try
            {
                var leadFilters = this.service.GetFilter(leadQuery);
                return new HttpResponse<LeadFilterDTO>
                {
                    IsStatusCodeSuccess = true,
                    data = leadFilters,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<LeadFilterDTO>
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
        [Route("linkUsers")]
        public HttpResponse<List<int>> LinkToUser([FromBody] List<int> userIds)
        {
            logger.LogInformation($"{this.GetType().Name} - LinkToUser");
            try
            {
                this.service.LinkWithoutOwnerToUser(userIds);
                return new HttpResponse<List<int>>
                {
                    IsStatusCodeSuccess = true,
                    data = userIds,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<int>>
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
