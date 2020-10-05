using System;
using System.Linq;
using System.Threading;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;

namespace KivalitaAPI.Services
{

    public class FlowTaskService : Service<FlowTask, KivalitaApiContext, FlowTaskRepository>
    {

        public readonly IJobScheduler _scheduler;

        public FlowTaskService(
            KivalitaApiContext context,
            FlowTaskRepository baseRepository,
            IJobScheduler scheduler
        ) : base(context, baseRepository)
        {
            _scheduler = scheduler;
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
        }

        public bool isJobAutomatic(FlowAction flowAction)
        {
            return flowAction.Type == "email" && flowAction.Flow.isAutomatic;
        }

        public TaskListDTO initialTasks(SieveModel filterQuery)
        {
            return this.baseRepository.initialTasks(filterQuery);
        }
    }
}
