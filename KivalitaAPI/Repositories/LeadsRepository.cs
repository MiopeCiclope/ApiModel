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
                return leadExists;
            }
            else
            {
                var lead = base.Add(entity);
                return lead;
            }

        }

        public override List<Leads> GetAll()
        {
            return context.Set<Leads>()
                .AsNoTracking()
                .Include(l => l.Company)
                .ToList();
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
