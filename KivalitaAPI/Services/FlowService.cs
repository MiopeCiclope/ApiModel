
using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
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

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            FlowActionRepository flowActionRepository,
            FlowTaskRepository flowTaskRepository,
            FilterRepository filterRepository,
            LeadsRepository leadsRepository
        ) : base(context, baseRepository) {
            _flowActionRepository = flowActionRepository;
            _flowTaskRepository = flowTaskRepository;
            _filterRepository = filterRepository;
            _leadsRepository = leadsRepository;
        }

        public override Flow Add(Flow flow)
        {
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
                WithoutEmail = filter.Email == "withoutEmail"
            };

            var leads = _leadsRepository.FetchFilterAll(leadQuery);

            foreach( var action in flow.FlowAction )
            {
                foreach (var lead in leads)
                {
                    var task = new FlowTask
                    {
                        Status = "pending",
                        LeadId = lead.Id,
                        FlowActionId = action.Id
                    };

                    _flowTaskRepository.Add(task);
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


