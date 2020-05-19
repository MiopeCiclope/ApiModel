using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Repositories
{
    public class LeadsRepository : Repository<Leads, DbContext>
    {
        public LeadsRepository(DbContext context) : base(context) {}

        public override Leads Add(Leads entity)
        {
            var leadSearch = this.GetBy(storedLead => storedLead.LinkedIn == entity.LinkedIn);
            var leadExists = leadSearch.Any() ? leadSearch.First() : null;
            if (leadExists != null)
            {
                return null;
            }
            else
            {
                var lead = base.Add(entity);
                return lead;
            }

        }

        public override List<Leads> AddRange(List<Leads> entities)
        {

            var missingRecords = entities.Where(x => !context.Set<Leads>().Any(z => z.LinkedIn == x.LinkedIn)).ToList();
            context.Set<Leads>().AddRange(missingRecords);
            context.SaveChanges();

            return missingRecords;
        }

        public override List<Leads> GetBy(Func<Leads, bool> condition)
        {
            var leads = base.GetBy(condition);
            leads = leads.Select(lead =>
            {
                return lead;
            }).ToList();
            return leads;
        }
    }
}
