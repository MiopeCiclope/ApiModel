
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class PreLeadRepository : Repository<PreLead, DbContext, SieveProcessor>
    {
        public PreLeadRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override PreLead Add(PreLead entity)
        {
            var leadSearch = this.GetBy(storedLead => storedLead.Email == entity.Email);
            var leadExists = leadSearch.Any() ? leadSearch.First() : null;
            if (leadExists != null)
            {
                return leadExists;
            }
            else
            {
                var lead = base.Add(entity);
                return lead;
            }

        }
    }
}

