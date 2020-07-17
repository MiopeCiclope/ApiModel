
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class FlowService : Service<Flow, KivalitaApiContext, FlowRepository>
    {

        FlowActionRepository _flowActionRepository;
        FlowTaskRepository _flowTaskRepository;
        FilterRepository _filterRepository;
        LeadsRepository _leadsRepository;
        public readonly IJobScheduler _scheduler;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            FlowActionRepository flowActionRepository,
            FlowTaskRepository flowTaskRepository,
            FilterRepository filterRepository,
            LeadsRepository leadsRepository,
            IJobScheduler scheduler
        ) : base(context, baseRepository) {
            _flowActionRepository = flowActionRepository;
            _flowTaskRepository = flowTaskRepository;
            _filterRepository = filterRepository;
            _leadsRepository = leadsRepository;
            _scheduler = scheduler;
        }

        public override Flow Add(Flow flow)
        {
            CancellationToken cancellationToken = new CancellationToken();
            var flowCreated = base.Add(flow);

            var filter = _filterRepository.Get(flow.FilterId);
            LeadQueryDTO leadQuery = new LeadQueryDTO
            {
                ItemsPerPage = 0,
                Page = 1,
                Sector = filter.Sector,
                Position = filter.Position,
                Company = filter.Company,
                WithEmail = filter.Email == "withEmail",
                WithoutEmail = filter.Email == "withoutEmail",
                UserId = filter.UserId
            };

            var leads = _leadsRepository.FetchFilterAll(leadQuery);

            foreach( var action in flow.FlowAction )
            {
                foreach (var lead in leads)
                {
                    var taskPayload = new FlowTask
                    {
                        Status = "pending",
                        LeadId = lead.Id,
                        FlowActionId = action.Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = flow.CreatedBy,
                        UpdatedAt = DateTime.Now
                    };

                    if (flow.FlowAction.First() == action)
                    {
                        taskPayload.ScheduledTo = DateTime.Now.AddMinutes(action.afterDays);
                    }

                    var task = _flowTaskRepository.Add(taskPayload);

                    if (task.ScheduledTo.HasValue)
                    {
                        DateTimeOffset dateTimeOffset = new DateTimeOffset((DateTime)task.ScheduledTo);
                        var job = new JobScheduleDTO("TaskJob", "0/2 * * * * ?", dateTimeOffset, task.Id);

                        _scheduler.ScheduleJob(cancellationToken, job);
                    }
                }
            }

            return flowCreated;
        }

        public override Flow Update(Flow flow)
        {
            var oldFlow = baseRepository.Get(flow.Id);

            var actionToUnlink = oldFlow.FlowAction.Where(flowAction => !flow.FlowAction.Select(flowAction => flowAction.Id)?.Contains(flowAction.Id) ?? true);
            var actionToLink = flow.FlowAction.Where(flowAction => !oldFlow.FlowAction?.Select(actionUnlink => actionUnlink.Id).Contains(flowAction.Id) ?? true);

            if (actionToUnlink.Any())
            {
                var actionList = actionToUnlink.ToList();
                _flowActionRepository.DeleteRange(actionList);
            }

            if (actionToLink.Any())
            {
                var actionList = actionToLink.ToList();
                actionList.ForEach(action => action.FlowId = flow.Id);
                _flowActionRepository.UpdateRange(actionList);
            }

            return base.Update(flow);
        }
    }
}


