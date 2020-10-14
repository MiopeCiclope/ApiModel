
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class LeadsDTORepository : Repository<LeadDatabaseDTO, DbContext, SieveProcessor>
    {
        public LeadsDTORepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

