
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class FlowTaskDTORepository : Repository<FlowTaskDTO, DbContext, SieveProcessor>
    {
        public FlowTaskDTORepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

