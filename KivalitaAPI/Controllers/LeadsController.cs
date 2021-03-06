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
using Sieve.Models;
using AutoMapper;
using KivalitaAPI.Interfaces;
using System.Threading;
using System.Linq;

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
        private readonly IMapper _mapper;
        public readonly IJobScheduler _scheduler;

        public LeadsController(
                CompanyService companyService
                , GetEmailService getEmailService
                , LeadsService _service
                , ILogger<LeadsController> logger
                , IMapper mapper
                , IJobScheduler scheduler
            )
        {
            this.service = _service;
            this.companyService = companyService;
            this._getEmailService = getEmailService;
            this.logger = logger;
            this._mapper = mapper;
            this._scheduler = scheduler;
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public virtual HttpResponse<Leads> Put(int id)
        {
            logger.LogInformation($"{this.GetType().Name} - Get - {id}");
            try
            {
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                var lead = service.Get(id);

                var statusRequest = (id != lead.Id) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = (statusRequest == HttpStatusCode.OK),
                    statusCode = statusRequest,
                    data = lead
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public virtual HttpResponse<Leads> Put(int id, Leads lead)
        {
            logger.LogInformation($"{this.GetType().Name} - Put - {id}");
            try
            {
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                lead.UpdatedBy = userAuditId;
                lead.UpdatedAt = DateTime.UtcNow;

                var updatedData = service.Update(lead);

                var statusRequest = (id != lead.Id) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = (statusRequest == HttpStatusCode.OK),
                    statusCode = statusRequest,
                    data = updatedData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpPut]
        [Authorize]
        [Route("BulkUpdate")]
        public virtual HttpResponse<bool> BulkUpdate([FromQuery] SieveModel filterQuery, [FromBody] LeadBulkDTO bulkLeadsOptions)
        {
            logger.LogInformation($"{this.GetType().Name} - BulkLeadUpdate");
            try
            {
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                bulkLeadsOptions.filter = filterQuery;
                var didUpdate = service.BulkUpdateLeads(bulkLeadsOptions);

                var statusRequest = didUpdate ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = (statusRequest == HttpStatusCode.OK),
                    statusCode = statusRequest,
                    data = true
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<bool>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = false,
                    ErrorMessage = "Erro ao realizar a requisi????o"
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }


        [HttpPost]
        //[Authorize]
        [Route("existsList")]
        public HttpResponse<List<string>> ExistList([FromBody] List<string> existList)
        {
            logger.LogInformation($"{this.GetType().Name} - Exists List");
            try
            {
                if(existList == null || existList.Count == 0)
                {
                    logger.LogError("Empty List");
                    return new HttpResponse<List<string>>
                    {
                        IsStatusCodeSuccess = false,
                        statusCode = HttpStatusCode.InternalServerError,
                        data = null,
                        ErrorMessage = "Erro ao realizar a requisi????o"
                    };
                }

                var existingLeads = this.service.LeadExists(existList);

                return new HttpResponse<List<string>>
                {
                    IsStatusCodeSuccess =  true,
                    statusCode = HttpStatusCode.OK,
                    data = existingLeads
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<List<string>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi????o"
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }


        [HttpPost]
        [Authorize]
        [Route("single")]
        public HttpResponse<Leads> Post([FromBody] Leads lead)
        {
            logger.LogInformation($"{this.GetType().Name} - Post Lead");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                var newLead = this.service.Add(lead);

                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = true,
                    data = newLead,
                    statusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi????o"
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

                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                leads.ForEach(lead => {
                    lead.CreatedBy = userAuditId;
                    lead.CreatedAt = utfNowTime;
                });

                List<Leads> newLeads = this.service.SaveRange(leads);

                var cancellationToken = new CancellationToken();
                var job = new JobScheduleDTO("FindMailJob", null, DateTimeOffset.UtcNow, 0);
                _scheduler.ScheduleJob(cancellationToken, job);

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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpGet]
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("v2")]
        public virtual HttpResponse<List<Leads>> GetAll_v2([FromQuery] SieveModel filterQuery)
        {
            logger.LogInformation($"{this.GetType().Name} - GetAll_v2");
            try
            {
                if (isColaborador()) filterQuery.Filters = $"{filterQuery.Filters},Owner=={GetAuditTrailUser()}";

                var dataList = service.GetAll_v2(filterQuery);

                return new HttpResponse<List<Leads>>
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
                return new HttpResponse<List<Leads>>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public virtual HttpResponse<Leads> Delete(int id)
        {
            logger.LogInformation($"{this.GetType().Name} - Delete - {id}");
            try
            {
                var userAuditId = GetAuditTrailUser();
                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                var statusRequest = HttpStatusCode.OK;
                var createdData = service.Delete(id, userAuditId);

                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = true,
                    statusCode = statusRequest,
                    data = createdData
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new HttpResponse<Leads>
                {
                    IsStatusCodeSuccess = false,
                    statusCode = HttpStatusCode.InternalServerError,
                    data = null,
                    ErrorMessage = String.IsNullOrEmpty(e.Message) ? "Erro ao realizar a requisi????o" : e.Message
                };
            }
        }

        [HttpDelete]
        [Authorize]
        public HttpResponse<string> DeleteList([FromBody] List<Leads> leadList)
        {
            logger.LogInformation($"{this.GetType().Name} - Delete Lead List");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                leadList.ForEach(lead => {
                    lead.Company = null;
                    lead.UpdatedBy = userAuditId;
                    lead.UpdatedAt = utfNowTime;
                });

                List<Leads> deletedLeads = this.service.DeleteRange(leadList);

                return new HttpResponse<string>
                {
                    IsStatusCodeSuccess = true,
                    data = "Leads deletadas com sucesso!",
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("import")]
        public HttpResponse<Boolean> ImpotLead([FromBody] List<Leads> leads)
        {
            logger.LogInformation($"{this.GetType().Name} - CSV Import");
            try
            {
                var userAuditId = GetAuditTrailUser();
                var utfNowTime = DateTime.UtcNow;

                if (userAuditId == 0) throw new Exception("Token Sem Usu??rio v??lido.");

                leads.ForEach(lead => {
                    lead.CreatedBy = userAuditId;
                    lead.CreatedAt = utfNowTime;
                });

                this.service.ImportLeads(leads);

                return new HttpResponse<Boolean>
                {
                    IsStatusCodeSuccess = true,
                    data = true,
                    statusCode = HttpStatusCode.OK
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
                    ErrorMessage = "Erro ao realizar a requisi????o"
                };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual bool isColaborador()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.FindFirst("Role").Value == "Colaborador";
        }
    }
}
