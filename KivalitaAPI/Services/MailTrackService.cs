
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class MailTrackService : Service<MailTrack, KivalitaApiContext, MailTrackRepository>
    {
        public MailTrackService(KivalitaApiContext context, MailTrackRepository baseRepository) : base(context, baseRepository) { }
    }
}


