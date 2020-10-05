
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;
using System.Collections.Generic;
using KivalitaAPI.Common;
using Sieve.Models;
using Microsoft.Linq.Translations;
using System;
using KivalitaAPI.DTOs;

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
                        .ThenInclude(f => f.Flow)
                        .ThenInclude(f => f.User)
                    .AsNoTracking();

            result = this.filterProcessor.Apply(filterQuery, result).WithTranslations();
            var total = 0;

            var response = result.ToList();
            return new QueryResult<FlowTask>
            {
                Items = response,
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

        public FlowTask GetCurrentTaskFromLead(int leadId, int flowId)
        {
            return context.Set<FlowTask>()
                .Include(f => f.FlowAction)
                .Where(
                    f => f.Status == "pending" &&
                    f.LeadId == leadId &&
                    f.FlowAction.FlowId == flowId
                )
                .Include(f => f.FlowAction)
                .FirstOrDefault();
        }

        public FlowTask GetNextTask(FlowTask currentTask)
        {
            return context.Set<FlowTask>()
                .Include(f => f.FlowAction)
                    .ThenInclude(fa => fa.Flow)
                .Where(
                    f => f.Status == "pending" &&
                    f.LeadId == currentTask.LeadId &&
                    f.Id != currentTask.Id &&
                    f.FlowAction.FlowId == currentTask.FlowAction.FlowId
                )
                .FirstOrDefault();
        }


        public TaskListDTO initialTasks(SieveModel filterQuery)
        {
            TaskListDTO separateTasks = new TaskListDTO();

            var result = context.Set<FlowTask>()
                    .Include(f => f.Leads)
                        .ThenInclude(l => l.Company)
                    .Include(f => f.FlowAction)
                    .AsNoTracking();

            var listResult = this.filterProcessor.Apply(filterQuery, result, applyPagination: false).WithTranslations().ToList();

            separateTasks.futureTasks = listResult.Where(task => task.ScheduledTo > DateTime.Now).Take(10).ToList();
            separateTasks.todayTasks = listResult.Where(task => task.ScheduledTo.Value.Date == DateTime.Now.Date).Take(10).ToList();
            separateTasks.overdueTasks = listResult.Where(task => task.ScheduledTo.Value.Date < DateTime.Now.Date).Take(10).ToList();

            return separateTasks;
        }

        public List<FlowTask> GetPendingByLead(int leadId)
        {
            return context.Set<FlowTask>()
                .Where(
                    f => f.LeadId == leadId &&
                    f.Status == "pending"
                )
                .AsNoTracking()
                .ToList();
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

        public List<FlowTask> GetSchedulableTask()
        {
            return context.Set<FlowTask>()
                            .Include(f => f.FlowAction)
                                .ThenInclude(fa => fa.Flow)
                            .WithTranslations()
                            .AsNoTracking()
                            .Where(task => task.Status == "pending" 
                                        && task.ScheduledTo.HasValue 
                                        && task.ScheduledTo.Value.Date <= DateTime.Now
                                        && task.FlowAction.Type == "email" 
                                        && task.FlowAction.Flow.isAutomatic
                                        && task.FlowAction.Flow.IsActive)
                            .ToList();
        }
    }
}
