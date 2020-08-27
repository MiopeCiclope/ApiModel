using KivalitaAPI.Common;
using KivalitaAPI.DTOs;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Linq.Translations;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class LeadsRepository : Repository<Leads, DbContext, SieveProcessor>
    {
        public const int DefaultItemsPerPage = 10;

        public LeadsRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) {}

        public override Leads Get(int id)
        {
            return context.Set<Leads>()
                .Where(l => l.Id == id)
                .Include(l => l.Flow)
                    .ThenInclude(f => f.User)
                .Include(l => l.Company)
                .Include(l => l.LeadTag)
                    .ThenInclude(l => l.Tag)
                .SingleOrDefault();
        }

        public Leads GetAsNoTracking(int id)
        {
            return context.Set<Leads>()
                .Include(l => l.LeadTag)
                    .ThenInclude(l => l.Tag)
                .Where(l => l.Id == id)
                .AsNoTracking()
                .SingleOrDefault();
        }

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
            int pageSize = leadQuery.ItemsPerPage ?? DefaultItemsPerPage;

            if (leadQuery.ItemsPerPage == 0) pageSize = totalItems;

            List<Leads> leads = queryable
                .Where(lead => 
                    lead.Deleted == false)
                .Skip((leadQuery.Page - 1) * pageSize)
                .Take(pageSize)
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
           return context.Set<Leads>()
                               .Include(l => l.Company)
                               .ThenInclude(c => c.User)
                               .WithTranslations()
                               .Where(condition)
                               .ToList();
        }

        private IQueryable<Leads> BuildQuery(IQueryable<Leads> queryable, LeadQueryDTO leadQuery)
        {

            if (!String.IsNullOrEmpty(leadQuery.Position))
            {
                queryable = queryable.Where(lead => lead.Position == leadQuery.Position);
            }
            if (!String.IsNullOrEmpty(leadQuery.Sector))
            {
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

        public new QueryResult<Leads> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Leads>()
                                .Include(l => l.Company)
                                    .ThenInclude(c => c.User)
                                .Include(l => l.LeadTag)
                                    .ThenInclude(l => l.Tag)
                                .WithTranslations()
                                .Where(lead => lead.Deleted == false)
                                .AsNoTracking();

            result = this.filterProcessor.Apply(filterQuery, result, applyPagination: false).WithTranslations();
            var total = result.Count();

            result = this.filterProcessor.Apply(filterQuery, result, applyFiltering: false, applySorting: false);
            var list = result.Select(l => new Leads
            {
                Id = l.Id,
                Name = l.Name,
                Position = l.Position,
                Email = l.Email,
                LinkedIn = l.LinkedIn,
                Status = l.Status,
                Flow = l.Flow,
                FlowId = l.FlowId,
                CreatedAt = l.CreatedAt,
                LeadTag = l.LeadTag,
                CompanyId = l.CompanyId,
                Company = new Company
                {
                    Id = l.Company.Id,
                    Name = l.Company.Name,
                    Sector = l.Company.Sector,
                    UserId = l.Company.UserId,
                    User = new User
                    {
                        Id = l.Company.User.Id,
                        FirstName = l.Company.User.FirstName,
                        LastName = l.Company.User.LastName
                    }
                }

            }).ToList();

            return new QueryResult<Leads>
            {
                Items = list,
                TotalItems = total,
            };
        }

        public override Leads Delete(int id, int userId)
        {
            var deletedLead = base.Get(id);
            deletedLead.UpdatedBy = userId;
            deletedLead.Deleted = true;
            return base.Update(deletedLead);
        }

        public override List<Leads> DeleteRange(List<Leads> leads)
        {
            leads.ForEach(l => l.Deleted = true);
            return base.UpdateRange(leads);
        }

        public void UpdateStatusList(List<int> Ids)
        {
            var query = $@"Update Leads set Status = {(int) LeadStatusEnum.Pending} where id in ({String.Join(',', Ids)})";
            context.Database.ExecuteSqlCommand(query);
        }

        public int GetAmountLeadsInFlow(int flowid)
        {
            return context.Set<Leads>()
                .Where(l => l.FlowId == flowid)
                .Count();
        }
    }
}
