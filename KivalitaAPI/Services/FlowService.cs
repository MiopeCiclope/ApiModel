
using System;
using System.Collections.Generic;
using System.Linq;
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
        ScheduleTasksService scheduleTasksService;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            LeadsRepository _leadsRepository,
            FlowActionRepository _flowActionRepository,
            FlowTaskRepository _flowTaskRepository,
            FilterRepository _filterRepository,
            TemplateRepository _templateRepository,
            MailAnsweredRepository _mailAnsweredRepository,
            ScheduleTasksService _scheduleTasksService
        ) : base(context, baseRepository) {
            leadsRepository = _leadsRepository;
            flowActionRepository = _flowActionRepository;
            flowTaskRepository = _flowTaskRepository;
            filterRepository = _filterRepository;
            templateRepository = _templateRepository;
            mailAnsweredRepository = _mailAnsweredRepository;
            scheduleTasksService = _scheduleTasksService;
        }

        public override Flow Add(Flow flow)
        {
            var filters = flow.Filter.Select(filter => filterRepository.Get(filter.Id)).ToList();
            flow.Filter = filters;

            var flowCreated = base.Add(flow);

            var filterIds = filters.Select(f => f.Id).ToList();
            var leads = GetLeadsByFilter(filterIds);
            leads = leads.Where(l => l.Status == LeadStatusEnum.ColdLead).ToList();

            scheduleTasksService.Execute(flowCreated, leads);

            return flowCreated;
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
                    filterRepository.UpdateRange(filterList);
                }

                if (filterToLink.Any())
                {
                    var filterList = filterToLink.ToList();
                    filterList.ForEach(filter => filter.FlowId = flow.Id);
                    filterRepository.UpdateRange(filterList);
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
                flowActionRepository.AddRange(actionList);
            }

            return base.Update(flow);
        }

        public override Flow Delete(int id, int userId)
        {
            if (HasTask(id))
            {
                throw new Exception("Não é possível excluir o Fluxo pois existe terefas relacionada a ele!");
            }

            return baseRepository.Delete(id, userId);
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

        public FlowReportDTO getReport(int flowId)
        {
            var amountOfLeads = leadsRepository.GetAmountLeadsInFlow(flowId);
            var amountSentEmails = flowTaskRepository.GetAmountSentEmails(flowId);
            var amountAnsweredEmails = mailAnsweredRepository.GetAmountAnsweredEmails(flowId);
            var amountPositiveAnsweredEmails = mailAnsweredRepository.GetAmountPositiveAnsweredEmails(flowId);
            var amountNegativeAnsweredEmails = mailAnsweredRepository.GetAmountNegativeAnsweredEmails(flowId);
            var amountNotFoundAnsweredEmails = mailAnsweredRepository.GetAmountNotFoundAnsweredEmails(flowId);
            return new FlowReportDTO
            {
                sentEmails = amountSentEmails,
                answeredEmails = amountAnsweredEmails,
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
                filterModel.Filters = filter.GetSieveFilter();
                var leadsFound = leadsRepository.GetAll_v2(filterModel).Items;

                leads.AddRange(leadsFound);
            }

            return leads;
        }

    }
}


