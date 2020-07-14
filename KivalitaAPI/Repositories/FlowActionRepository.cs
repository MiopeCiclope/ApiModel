
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class FlowActionRepository : Repository<FlowAction, DbContext, SieveProcessor>
    {
        public FlowActionRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

