using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Repositories
{
    public class LeadsRepository : Repository<Leads, DbContext>
    {
        public LeadsRepository(DbContext context) : base(context) { }

        public override Leads Get(int id)
        {
            var lead = base.Get(id);
            return lead;
        }

        public override List<Leads> GetAll()
        {
            var leads = base.GetAll();
            leads = leads.Select(lead =>
            {
                return lead;
            }).ToList();
            return leads;
        }

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

        public override Leads Update(Leads entity)
        {
            var lead = base.Update(entity);
            return lead;
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
