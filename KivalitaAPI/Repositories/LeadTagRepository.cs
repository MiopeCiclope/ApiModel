
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class LeadTagRepository : Repository<LeadTag, DbContext, SieveProcessor>
    {
        public LeadTagRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

