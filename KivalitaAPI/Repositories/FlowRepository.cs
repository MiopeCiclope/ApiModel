
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Common;
using Sieve.Models;

namespace KivalitaAPI.Repositories
{
    public class FlowRepository : Repository<Flow, DbContext, SieveProcessor>
    {
        public FlowRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override List<Flow> GetAll()
        {
            return context.Set<Flow>()
                .Include(f => f.FlowAction)
                .Include(f => f.User)
                .AsNoTracking()
                .ToList();
        }

        public override QueryResult<Flow> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Flow>()
                .Include(f => f.User)
                .AsNoTracking();

            var total = result.Count();
            result = this.filterProcessor.Apply(filterQuery, result);

            return new QueryResult<Flow>
            {
                Items = result.ToList(),
                TotalItems = total,
            };
        }

        public override Flow Get(int id)
        {
            return context.Set<Flow>()
                .Where(f => f.Id == id)
                .Include(f => f.FlowAction)
                .Include(f => f.Filter)
                .SingleOrDefault();
        }
    }
}

