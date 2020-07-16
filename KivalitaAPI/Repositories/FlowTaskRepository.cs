
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class FlowTaskRepository : Repository<FlowTask, DbContext, SieveProcessor>
    {
        public FlowTaskRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

