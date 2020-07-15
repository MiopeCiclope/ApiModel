
using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class FlowService : Service<Flow, KivalitaApiContext, FlowRepository>
    {

        FlowActionRepository _flowActionRepository;

        public FlowService(
            KivalitaApiContext context,
            FlowRepository baseRepository,
            FlowActionRepository flowActionRepository
        ) : base(context, baseRepository) {
            _flowActionRepository = flowActionRepository;
        }

        public override Flow Add(Flow flow)
        {
            return base.Add(flow);
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


