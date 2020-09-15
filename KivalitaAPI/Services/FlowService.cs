
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
        ScheduleTasksService scheduleTasksService;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            LeadsRepository _leadsRepository,
            FlowActionRepository _flowActionRepository,
            FlowTaskRepository _flowTaskRepository,
            FilterRepository _filterRepository,
            TemplateRepository _templateRepository,
            ScheduleTasksService _scheduleTasksService
        ) : base(context, baseRepository) {
            leadsRepository = _leadsRepository;
            flowActionRepository = _flowActionRepository;
            flowTaskRepository = _flowTaskRepository;
            filterRepository = _filterRepository;
            templateRepository = _templateRepository;
            scheduleTasksService = _scheduleTasksService;
        }

        public override Flow Add(Flow flow)
        {
            var flowCreated = base.Add(flow);

            var leads = GetLeadsByFilter(flow.FilterId);
            leads = leads.Where(l => l.Status == LeadStatusEnum.ColdLead).ToList();

            scheduleTasksService.Execute(flowCreated, leads);

            return flowCreated;
        }

        private List<Leads> GetLeadsByFilter(int filterId)
        {
            var filter = filterRepository.Get(filterId);
            var filterModel = new SieveModel();
            filterModel.Filters = filter.GetSieveFilter();

            return leadsRepository.GetAll_v2(filterModel).Items;
        }


        public override Flow Update(Flow flow)
        {
            var oldFlow = baseRepository.Get(flow.Id);

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
                flowActionRepository.UpdateRange(actionList);
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
            return new FlowReportDTO
            {
                sentEmails = 0,
                answeredEmails = 0,
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

    }
}


