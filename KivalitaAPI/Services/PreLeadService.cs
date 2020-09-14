
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class PreLeadService : Service<PreLead, KivalitaApiContext, PreLeadRepository>
    {
        public PreLeadService(KivalitaApiContext context, PreLeadRepository baseRepository) : base(context, baseRepository) { }
    }
}


