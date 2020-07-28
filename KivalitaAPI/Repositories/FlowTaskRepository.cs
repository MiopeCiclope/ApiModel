
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;
using System.Collections.Generic;
using KivalitaAPI.Common;
using Sieve.Models;
using Microsoft.Linq.Translations;
using System;

namespace KivalitaAPI.Repositories
{
    public class FlowTaskRepository : Repository<FlowTask, DbContext, SieveProcessor>
    {
        public FlowTaskRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override List<FlowTask> GetAll()
        {
            return context.Set<FlowTask>()
                .Include(f => f.Leads)
                    .ThenInclude(l => l.Company)
                .Include(f => f.FlowAction)
                .WithTranslations()
                .AsNoTracking()
                .ToList();
        }

        public override QueryResult<FlowTask> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<FlowTask>()
                .Include(f => f.Leads)
                    .ThenInclude(l => l.Company)
                .Include(f => f.FlowAction)
                .AsNoTracking();
            var total = result.Count();

            result = this.filterProcessor.Apply(filterQuery, result).WithTranslations();

            return new QueryResult<FlowTask>
            {
                Items = result.ToList(),
                TotalItems = total,
            };
        }

        public override FlowTask Get(int id)
        {
            return context.Set<FlowTask>()
                .Where(f => f.Id == id)
                .Include(f => f.TaskNote)
                .Include(f => f.Leads)
                    .ThenInclude(l => l.Company)
                .Include(f => f.FlowAction)
                    .ThenInclude(fa => fa.Flow)
                .WithTranslations()
                .AsNoTracking()
                .SingleOrDefault();
        }

        public FlowTask GetNextTask(FlowTask currentTask)
        {
            return context.Set<FlowTask>()
                .Where(
                    f => f.Status == "pending" &&
                    f.LeadId == currentTask.LeadId &&
                    f.Id != currentTask.Id
                )
                .Include(f => f.FlowAction)
                .FirstOrDefault();
        }

        public override List<FlowTask> GetBy(Func<FlowTask, bool> condition)
        {
            return context.Set<FlowTask>()
                            .Include(f => f.TaskNote)
                            .Include(f => f.Leads)
                                .ThenInclude(l => l.Company)
                            .Include(f => f.FlowAction)
                                .ThenInclude(fa => fa.Flow)
                            .WithTranslations()
                            .AsNoTracking()
                            .Where(condition)
                            .ToList();
        }
    }
}
