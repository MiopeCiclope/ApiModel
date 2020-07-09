
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class FlowRepository : Repository<Flow, DbContext, SieveProcessor>
    {
        public FlowRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

