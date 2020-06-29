
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;

namespace KivalitaAPI.Services
{

    public class WpRdStationService : Service<WpRdStation, KivalitaApiContext, WpRdStationRepository>
    {
        public WpRdStationService(KivalitaApiContext context, WpRdStationRepository baseRepository) : base(context, baseRepository) { }

        public override WpRdStation Add(WpRdStation integration)
        {
            integration.CreatedAt = DateTime.UtcNow;
            return base.Add(integration);
        }
    }
}


