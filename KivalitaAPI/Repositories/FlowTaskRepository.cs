
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
using AutoMapper;

namespace KivalitaAPI.Repositories
{
    public class FlowTaskRepository : Repository<FlowTask, DbContext, SieveProcessor>
    {
        private readonly IMapper _mapper;
        public FlowTaskRepository(DbContext context, SieveProcessor filterProcessor, IMapper mapper) : base(context, filterProcessor) {
            _mapper = mapper;
        }

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

        public QueryResult<FlowTask> GetTask(string type, int userId, int take, int skip, string date = null, SieveModel filterQuery = null)
        {
            var query = BuildTaskQuery(userId, type, take, skip, date, filterQuery);
            var response = context.Set<FlowTaskQueryDTO>().FromSqlRaw(query).ToList();
            var result = _mapper.Map<List<FlowTask>>(response);

            return new QueryResult<FlowTask>
            {
                Items = result,
                TotalItems = response.Any() ? response.First().Total : 0,
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
                                        && task.ScheduledTo.Value.Date <= DateTime.Now.Date
                                        && task.FlowAction.Type == "email" 
                                        && task.FlowAction.Flow.isAutomatic
                                        && task.FlowAction.Flow.IsActive)
                            .ToList();
        }

        public int GetAmountSentEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<FlowTask>()
                .Where(
                    task => task.FlowAction.Type == "email"
                    && task.FlowAction.FlowId == flowid
                    && task.Status == "finished"
                );

            if (templateId.HasValue)
                query = query.Where(task => task.FlowAction.TemplateId == templateId);

            return query.Count();
        }

        private string BuildTaskQuery(int userId, string taskType, int take, int skip, string date = null, SieveModel filterQuery = null)
        {
            var today = DateTime.Now.Date;
            var ownerCondition = userId == 0 ? "" : $"and[c].[UserId] = { userId}";
            var statusCondition = $"and [f].Status = 'pending'";

            var aditionalQuery = $"";
            if (filterQuery != null && filterQuery.Filters != null && filterQuery.GetFiltersParsed().Where(filter => filter.Names.Contains("actionType")).Any())
            {
                var filters = filterQuery.GetFiltersParsed().First();
                aditionalQuery = $@"and [f0].type = '{filters.Values.First()}'";
            }

            var dateCondition = "";
            switch (taskType)
            {
                case "today":
                    dateCondition = $@"[f].[ScheduledTo] between '{today.ToString("yyyy/MM/dd")}' 
                                            and '{today.AddDays(1).ToString("yyyy/MM/dd")}'";
                    break;
                case "overdue":
                    dateCondition = $@"[f].[ScheduledTo] < '{today.ToString("yyyy/MM/dd")}'";
                    break;
                case "future":
                    dateCondition = $@"[f].[ScheduledTo] between '{today.AddDays(1).ToString("yyyy/MM/dd")}' 
                                            and '{today.AddDays(3).ToString("yyyy/MM/dd")}'";
                    break;
                case "finished":
                    var selectedDate = Convert.ToDateTime(date);
                    dateCondition = $@"[f].[ScheduledTo] between '{selectedDate.ToString("yyyy/MM/dd")}' 
                                            and '{selectedDate.AddDays(1).ToString("yyyy/MM/dd")}'";
                    statusCondition = $"and [f].Status = 'finished'";
                    break;
                default:
                    dateCondition = $@"[f].[ScheduledTo] between '{today.ToString("yyyy/MM/dd")}' 
                                            and '{today.AddDays(1).ToString("yyyy/MM/dd")}'";
                    break;
            }

            var query = $@"SELECT  [f].[Id], 
                                [f].[CreatedAt], 
                                [f].[CreatedBy], 
                                [f].[FlowActionId], 
                                [f].[LeadId], 
                                [f].[ScheduledTo], 
                                [f].[Status], 
                                [f].[UpdatedAt], 
                                [f].[UpdatedBy], 
                                [l].[Id] as LeadsId,
                                [l].[Name] as LeadName,
                                [l].[Phone] as LeadPhone,
                                [l].[LinkedInPublic] as LeadLinkedIn,
                                [c].[Id] as CompanyId, 
		                        [c].[UserId] as Owner, 
                                [c].[Name] as CompanyName, 
                                [f0].[FlowId] as FlowActionFlowId, 
                                [f0].[Type] as FlowActionType, 
                                [f1].[Id] as FlowId, 
                                [f1].[Name] as FlowName, 
                                [u].[Id] as UserId, 
                                [u].[FirstName] as UserFirstName,
                                Total = COUNT(*) OVER()
                        FROM       [FlowTask]   AS [f] 
                        LEFT JOIN  [FlowAction] AS [f0] 
                        ON         [f].[FlowActionId] = [f0].[Id] 
                        LEFT JOIN  [Flow] AS [f1] 
                        ON         [f0].[FlowId] = [f1].[Id] 
                        INNER JOIN [Leads] AS [l] 
                        ON         [f].[LeadId] = [l].[Id] 
                        LEFT JOIN  [Company] AS [c] 
                        ON         [l].[CompanyId] = [c].[Id] 
                        LEFT JOIN  [user] [u] 
                        ON         u.id = [c].userid 
                        WHERE  
                            {dateCondition}
	                        {ownerCondition}
                            {statusCondition}
                            {aditionalQuery}
	                        and [f1].[IsActive] = 1
                            AND (
			                        ([f0].[Type] = N'email' AND [f1].[isAutomatic] <> 1) 
			                        OR [f0].[Type] <> N'email'
		                        )
                        ORDER BY   (SELECT 1) offset {skip} rows FETCH next {take} rows only";
            return query;
        }
    }
}
