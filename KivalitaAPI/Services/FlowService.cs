
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class FlowService : Service<Flow, KivalitaApiContext, FlowRepository>
    {
        public FlowService(KivalitaApiContext context, FlowRepository baseRepository) : base(context, baseRepository) { }
    }
}


