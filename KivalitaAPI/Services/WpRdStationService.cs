
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class WpRdStationService : Service<WpRdStation, KivalitaApiContext, WpRdStationRepository>
    {
        public WpRdStationService(KivalitaApiContext context, WpRdStationRepository baseRepository) : base(context, baseRepository) { }
    }
}


