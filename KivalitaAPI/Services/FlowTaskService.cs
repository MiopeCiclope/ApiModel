using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using AutoMapper;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;

namespace KivalitaAPI.Services
{

    public class FlowTaskService : Service<FlowTask, KivalitaApiContext, FlowTaskRepository>
    {

        public readonly IJobScheduler _scheduler;
        private readonly IMapper _mapper;
        private readonly FlowTaskDTORepository _flowtaskDtoRepository;
        private readonly LeadsRepository _leadsRepository;
        private readonly FlowActionRepository _flowActionRepository;
        private readonly FlowTaskRepository _flowTaskRepository;

        public FlowTaskService(
            KivalitaApiContext context,
            FlowTaskRepository baseRepository,
            IJobScheduler scheduler,
            IMapper mapper,
            FlowTaskDTORepository flowtaskDtoRepository,
            LeadsRepository leadsRepository,
            FlowActionRepository flowActionRepository,
            FlowTaskRepository flowTaskRepository
        ) : base(context, baseRepository)
        {
            _scheduler = scheduler;
            _mapper = mapper;
            _flowtaskDtoRepository = flowtaskDtoRepository;
            _leadsRepository = leadsRepository;
            _flowActionRepository = flowActionRepository;
            _flowTaskRepository = flowTaskRepository;
        }

        public override FlowTask Update(FlowTask flowTask)
        {
            var oldFlowTask = baseRepository.Get(flowTask.Id);
            if (oldFlowTask.Status == "pending" && flowTask.Status == "finished")
            {
                scheduleNextTask(flowTask);
            }

            return baseRepository.Update(flowTask);
        }

        public override FlowTask Delete(int id, int userId)
        {
            var flowTask = baseRepository.Get(id);

            scheduleNextTask(flowTask);

            baseRepository.Delete(id, userId);

            return flowTask;
        }

        public void scheduleNextTask(FlowTask currentTask)
        {
            var cancellationToken = new CancellationToken();
            var nextFlowTask = baseRepository.GetNextTask(currentTask);

            if (nextFlowTask != null)
            {
                var nextFlowAction = nextFlowTask.FlowAction;
                var flow = nextFlowAction.Flow;

                var daysAllowedToSchedule = flow.DaysOfTheWeek.Split(',').Select(Int32.Parse).ToList();


                var date = DateTime.Now.AddDays(nextFlowAction.afterDays);
                date = DateUtils.GetDateSheduleValid(date, daysAllowedToSchedule);
                nextFlowTask.ScheduledTo = date;
                baseRepository.Update(nextFlowTask);

                if (isJobAutomatic(nextFlowAction))
                {

                    DateTimeOffset dateTimeOffset = new DateTimeOffset((DateTime)nextFlowTask.ScheduledTo);
                    var job = new JobScheduleDTO("TaskJob", "0/2 * * * * ?", dateTimeOffset, nextFlowTask.Id);

                    _scheduler.ScheduleJob(cancellationToken, job);
                }
            } 
            else
                afterFlowAction(currentTask);
        }

        private void afterFlowAction(FlowTask task)
        {
            updateLeadStatus(task.LeadId);
            if(task.FlowAction.Type != "doneflow")
                createdFlowDoneTask(task);
        }

        private void createdFlowDoneTask(FlowTask task)
        {
            var doneFlowActionQuery = _flowActionRepository
                            .GetBy(action => action.FlowId == task.FlowAction.FlowId && action.Type == "doneflow");

            if (doneFlowActionQuery.Any())
            {
                var doneFlowAction = doneFlowActionQuery.First();
                createTask(doneFlowAction.Id, task.LeadId);
            }
            else
            {
                var flowAction = _flowActionRepository.Add(new FlowAction
                {
                    afterDays = 0,
                    Done = false,
                    TemplateId = 0,
                    Type = "doneflow",
                    CreatedBy = 6,
                    FlowId = task.FlowAction.FlowId
                });
                createTask(flowAction.Id, task.LeadId);
            }
        }

        private void createTask(int flowActionId, int leadId)
        {
            _flowTaskRepository.Add(new FlowTask
            {
                FlowActionId = flowActionId,
                LeadId = leadId,
                CreatedBy = 6,
                Status = "pending",
                ScheduledTo = DateTime.UtcNow,
            });
        }

        private void updateLeadStatus(int leadId)
        {
            var lead = _leadsRepository.Get(leadId);
            lead.Status = LeadStatusEnum.Pending;
            _leadsRepository.Update(lead);
        }

        public bool isJobAutomatic(FlowAction flowAction)
        {
            return flowAction.Type == "email" && flowAction.Flow.isAutomatic;
        }

        public List<FlowTask> SaveRange(List<FlowTask> tasks)
        {
            this.baseRepository.AddRange(tasks);
            return tasks;
        }

        public TaskListDTO initialTasks(SieveModel filterQuery)
        {
            return this.baseRepository.initialTasks(filterQuery);
        }

        public List<FlowTask> UpdateBulk(List<FlowTask> tasks)
        {
            var flowTaskDto = _mapper.Map<List<FlowTaskDatabaseDTO>>(tasks);
            this._flowtaskDtoRepository.UpdateRange(flowTaskDto);
            return tasks;
        }

        public QueryResult<FlowTask> GetTask(string type, int userId = 0, int take = 20, int skip = 0, string date = null, SieveModel filterQuery = null)
        {
            return this.baseRepository.GetTask(type, userId, take, skip, date, filterQuery);
        }
    }
}
