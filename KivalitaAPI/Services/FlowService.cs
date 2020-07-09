
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
    }
}


