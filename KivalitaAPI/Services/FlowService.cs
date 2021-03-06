
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;

namespace KivalitaAPI.Services
{

    public class FlowService : Service<Flow, KivalitaApiContext, FlowRepository>
    {
        LeadsRepository leadsRepository;
        FlowActionRepository flowActionRepository;
        FlowTaskRepository flowTaskRepository;
        FilterRepository filterRepository;
        TemplateRepository templateRepository;
        MailAnsweredRepository mailAnsweredRepository;
        MailTrackRepository mailTrackRepository;
        ScheduleTasksService scheduleTasksService;
        IMapper mapper;
        FilterDTORepository filterDTORepository;
        LeadsDTORepository leadsDTORepository;
        FlowTaskDTORepository taskDtoRepository;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            LeadsRepository _leadsRepository,
            FlowActionRepository _flowActionRepository,
            FlowTaskRepository _flowTaskRepository,
            FilterRepository _filterRepository,
            TemplateRepository _templateRepository,
            ScheduleTasksService _scheduleTasksService,
            IMapper _mapper,
            FilterDTORepository _filterDTORepository,
            MailAnsweredRepository _mailAnsweredRepository,
            MailTrackRepository _mailTrackRepository,
            LeadsDTORepository _leadsDTORepository,
            FlowTaskDTORepository _taskDtoRepository
        ) : base(context, baseRepository) {
            leadsRepository = _leadsRepository;
            flowActionRepository = _flowActionRepository;
            flowTaskRepository = _flowTaskRepository;
            filterRepository = _filterRepository;
            templateRepository = _templateRepository;
            mailAnsweredRepository = _mailAnsweredRepository;
            mailTrackRepository = _mailTrackRepository;
            scheduleTasksService = _scheduleTasksService;
            mapper = _mapper;
            filterDTORepository = _filterDTORepository;
            leadsDTORepository = _leadsDTORepository;
            taskDtoRepository = _taskDtoRepository;
        }

        public override Flow Add(Flow flow)
        {
            var hasFilter = flow.Filter?.Any() ?? false;

            if (hasFilter)
            {
                var filters =  flow.Filter.Select(filter => filterRepository.Get(filter.Id)).ToList();
                flow.Filter = filters;
                var flowCreated = base.Add(flow);

                var filterIds = filters.Select(f => f.Id).ToList();
                var leads = GetLeadsByFilter(filterIds);
                leads = leads.Where(l => l.Status != LeadStatusEnum.Blacklist).ToList();
                scheduleTasksService.Execute(flowCreated, leads);
             
                return flowCreated;
            } else
            {
                var flowCreated = base.Add(flow);
                return flowCreated;
            }
        }

        public override Flow Update(Flow flow)
        {
            var oldFlow = baseRepository.Get(flow.Id);

            // Update Filters
            if (flow.Filter != null)
            {
                var filterToUnlink = oldFlow.Filter.Where(filter => !flow.Filter.Select(filter => filter.Id)?.Contains(filter.Id) ?? true);
                var filterToLink = flow.Filter.Where(filter => !oldFlow.Filter?.Select(filterUnlink => filterUnlink.Id).Contains(filter.Id) ?? true);
                if (filterToUnlink.Any())
                {
                    var filterList = filterToUnlink.ToList();
                    filterList.ForEach(filter => filter.FlowId = null);

                    var bulkListFilter = mapper.Map<List<FilterDatabaseDTO>>(filterList);
                    filterDTORepository.UpdateRange(bulkListFilter);
                }

                if (filterToLink.Any())
                {
                    var filterList = filterToLink.ToList();
                    filterList.ForEach(filter => filter.FlowId = flow.Id);

                    var bulkListFilter = mapper.Map<List<FilterDatabaseDTO>>(filterList);
                    filterDTORepository.UpdateRange(bulkListFilter);
                }

                var filterUnlinkIds = filterToUnlink.Select(f => f.Id).ToList();
                var leadsUnlink = GetLeadsByFilter(filterUnlinkIds);
                scheduleTasksService.RemoveRange(leadsUnlink);

                var filterLinkIds = filterToLink.Select(f => f.Id).ToList();
                var leadsLink = GetLeadsByFilter(filterLinkIds);
                scheduleTasksService.Execute(flow, leadsLink);
            }

            // Update Flow Actions
            var actionToUnlink = oldFlow.FlowAction.Where(flowAction => !flow.FlowAction.Select(flowAction => flowAction.Id)?.Contains(flowAction.Id) ?? true);
            var actionToLink = flow.FlowAction.Where(flowAction => !oldFlow.FlowAction?.Select(actionUnlink => actionUnlink.Id).Contains(flowAction.Id) ?? true);

            if (actionToUnlink.Any())
            {
                var actionList = actionToUnlink.ToList();
                flowActionRepository.DeleteRange(actionList);
            }

            if (actionToLink.Any())
            {
                var actionList = actionToLink.ToList();
                actionList.ForEach(action => action.FlowId = flow.Id);
                flowActionRepository.AddRangeNoBulk(actionList);
            }

            return base.Update(flow);
        }

        public override Flow Delete(int id, int userId)
        {
            this.RemoveFlowFromFilter(id);
            this.RemoveFlowFromLeads(id);
            this.RemoveFlowTasks(id);
            return baseRepository.Delete(id, userId);
        }

        private void RemoveFlowTasks(int id)
        {
            var flowActions = this.flowActionRepository.GetAll().Where(action => action.FlowId == id);
            if(flowActions.Any())
            {
                var actionIds = flowActions.Select(action => action.Id);
                var flowTasks = this.flowTaskRepository.GetBy(task => actionIds.Contains(task.FlowActionId.Value) && task.Status == "pending");

                if(flowTasks.Any())
                {
                    foreach (var task in flowTasks)
                    {
                        task.Status = "finished";
                    }
                    var bulkListTask = mapper.Map<List<FlowTaskDatabaseDTO>>(flowTasks);
                    taskDtoRepository.UpdateRange(bulkListTask);
                }
            }
        }

        private void RemoveFlowFromFilter(int id)
        {
            var flowFilter = this.filterRepository.GetBy(filter => filter.FlowId == id);
            if(flowFilter.Any())
            {
                foreach (var filter in flowFilter)
                {
                    filter.FlowId = null;

                }
                var bulkListFilter = mapper.Map<List<FilterDatabaseDTO>>(flowFilter);
                filterDTORepository.UpdateRange(bulkListFilter);
            }
        }

        private void RemoveFlowFromLeads(int id)
        {
            var filterLeads = this.leadsRepository.GetLeadsByFlowId(id, null);
            if (filterLeads.Any())
            { 
                foreach (var lead in filterLeads)
                {
                    lead.Status = LeadStatusEnum.Paused;
                    lead.FlowId = null;
                }
                var bulkListLeads = mapper.Map<List<LeadDatabaseDTO>>(filterLeads);
                leadsDTORepository.UpdateRange(bulkListLeads);
            }
        }

        public bool HasTemplate(int templateId)
        {
            var hasTemplate = flowActionRepository.GetBy(f => f.TemplateId == templateId);

            return hasTemplate.FirstOrDefault() != null;
        }

        public bool HasTask(int flowId)
        {
            var hasTask = flowActionRepository.GetBy(f => f.FlowId == flowId);

            return hasTask.FirstOrDefault() != null;
        }

        public FlowReportDTO getReport(int flowId, int? templateId = null)
        {
            var amountOfLeads = leadsRepository.GetAmountLeadsInFlow(flowId);
            var amountSentEmails = flowTaskRepository.GetAmountSentEmails(flowId, templateId);
            var amountAnsweredEmails = mailAnsweredRepository.GetAmountAnsweredEmails(flowId, templateId);
            var amountOpenedEmails = mailTrackRepository.GetAmountOpenedEmails(flowId, templateId);
            var amountPositiveAnsweredEmails = mailAnsweredRepository.GetAmountPositiveAnsweredEmails(flowId, templateId);
            var amountNegativeAnsweredEmails = mailAnsweredRepository.GetAmountNegativeAnsweredEmails(flowId, templateId);
            var amountNotFoundAnsweredEmails = mailAnsweredRepository.GetAmountNotFoundAnsweredEmails(flowId, templateId);
            return new FlowReportDTO
            {
                sentEmails = amountSentEmails,
                answeredEmails = amountAnsweredEmails,
                openedEmails = amountOpenedEmails,
                positiveAnsweredEmails = amountPositiveAnsweredEmails,
                negativeAnsweredEmails = amountNegativeAnsweredEmails,
                notFoundAnsweredEmails = amountNotFoundAnsweredEmails,
                amountOfLeads = amountOfLeads
            };
        }

        public List<FlowLeadsDTO> getLeads(int flowId, SieveModel filterQuery)
        {
            var leads = leadsRepository.GetLeadsByFlowId(flowId, filterQuery);
            var templates = templateRepository.GetAllAsNoTracking();

            return leads.Select(lead => {
                var task = flowTaskRepository.GetCurrentTaskFromLead(lead.Id, flowId);
                var template = templates.Find(t => t.Id == task?.FlowAction?.TemplateId);

                var flowTask = task != null
                    ? new FlowTaskDTO { Type = task.FlowAction.Type, TemplateName = template?.Name }
                    : null;

                return new FlowLeadsDTO
                {
                    Id = lead.Id,
                    Name = lead.Name,
                    Email = lead.Email,
                    Task = flowTask
                };
            }).ToList();
        }

        private List<Leads> GetLeadsByFilter(List<int> filterIds)
        {
            var leads = new List<Leads> { };
            foreach (var filterId in filterIds)
            {
                var filter = filterRepository.Get(filterId);
                var filterModel = new SieveModel();

                filterModel.Filters = string.IsNullOrEmpty(filter.SieveFilter)
                    ? filter.GetSieveFilter()
                    : filter.SieveFilter;

                var leadsFound = leadsRepository.GetAll_v2(filterModel).Items;

                leads.AddRange(leadsFound);
            }

            return leads
                    .OrderBy(lead => lead.Company.Name)
                    .ToList();
        }

    }
}


