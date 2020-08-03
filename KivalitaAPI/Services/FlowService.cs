
using System;
using System.Linq;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class FlowService : Service<Flow, KivalitaApiContext, FlowRepository>
    {

        FlowActionRepository flowActionRepository;
        ScheduleTasksService scheduleTasksService;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            FlowActionRepository _flowActionRepository,
            ScheduleTasksService _scheduleTasksService
        ) : base(context, baseRepository) {
            flowActionRepository = _flowActionRepository;
            scheduleTasksService = _scheduleTasksService;
        }

        public override Flow Add(Flow flow)
        {
            var flowCreated = base.Add(flow);
            scheduleTasksService.Execute(flowCreated);

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
    }
}


