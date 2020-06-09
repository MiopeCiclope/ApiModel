using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Common;
using KivalitaAPI.DTOs;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Repositories
{
    public class LeadsRepository : Repository<Leads, DbContext>
    {
        public const int ItemsPerPage = 10;

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

        public QueryResult<Leads> FetchAll(LeadQueryDTO leadQuery)
        {
            IQueryable<Leads> queryable = context.Set<Leads>()
                .Include(l => l.Company)
                .ThenInclude(c => c.User)
                .AsNoTracking();

            queryable = BuildQuery(queryable, leadQuery);

            int totalItems = queryable.Count();

            List<Leads> leads = queryable
                .Skip((leadQuery.Page - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            return new QueryResult<Leads>
            {
                Items = leads,
                TotalItems = totalItems,
            };
        }

        public List<Leads> FetchFilterAll(LeadQueryDTO leadQuery)
        {
            IQueryable<Leads> queryable = context.Set<Leads>()
                .Include(l => l.Company)
                .AsNoTracking();

            queryable = BuildQuery(queryable, leadQuery);

            return queryable.ToList();
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

        private IQueryable<Leads> BuildQuery(IQueryable<Leads> queryable, LeadQueryDTO leadQuery)
        {

            if (!String.IsNullOrEmpty(leadQuery.Position))
            {
                queryable = queryable.Where(lead => lead.Position == leadQuery.Position);
            }
            if (!String.IsNullOrEmpty(leadQuery.Sector))
            {
                Console.WriteLine($"Sector: {leadQuery.Sector}");
                queryable = queryable.Where(lead => lead.Company.Sector == leadQuery.Sector);
            }
            if (!String.IsNullOrEmpty(leadQuery.Company))
            {
                queryable = queryable.Where(lead => lead.Company.Name == leadQuery.Company);
            }
            if (leadQuery.Date.HasValue)
            {
                queryable = queryable.Where(lead => lead.CreatedAt.Date == leadQuery.Date);
            }
            if (leadQuery.UserId.HasValue)
            {
                var userId = leadQuery.UserId == 0 ? null : leadQuery.UserId;
                queryable = queryable.Where(lead => lead.Company.UserId == userId);
            }
            if (!String.IsNullOrEmpty(leadQuery.Search))
            {
                queryable = queryable.Where(
                    lead => lead.Name.Contains(leadQuery.Search) ||
                            lead.Email.Contains(leadQuery.Search));
            }
            if (leadQuery.WithEmail)
            {
                queryable = queryable.Where(lead => lead.Email != null);
            }
            else if (leadQuery.WithoutEmail)
            {
                queryable = queryable.Where(lead => lead.Email == null);
            }

            return queryable;
        }
    }
}
