
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class WpRdStationRepository : Repository<WpRdStation, DbContext, SieveProcessor>
    {
        public WpRdStationRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

