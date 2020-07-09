
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class FlowRepository : Repository<Flow, DbContext, SieveProcessor>
    {
        public FlowRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override List<Flow> GetAll()
        {
            return context.Set<Flow>()
                .Include(f => f.FlowAction)
                .AsNoTracking()
                .ToList();
        }

        public override Flow Get(int id)
        {
            return context.Set<Flow>()
                .Where(f => f.Id == id)
                .Include(f => f.FlowAction)
                .SingleOrDefault();
        }
    }
}

