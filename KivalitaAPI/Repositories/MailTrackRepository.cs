
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class MailTrackRepository : Repository<MailTrack, DbContext, SieveProcessor>
    {
        public MailTrackRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

