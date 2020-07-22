using System;
using System.Linq;
using System.Threading;
using KivalitaAPI.Data;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public void scheduleNextTask(FlowTask currentTask)
        {
            var cancellationToken = new CancellationToken();
            var nextFlowTask = baseRepository.GetNextTask(currentTask);

            if (nextFlowTask != null)
            {
                var nextFlowAction = nextFlowTask.FlowAction;

                nextFlowTask.ScheduledTo = DateTime.Now.AddDays(nextFlowAction.afterDays);
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
            return flowAction.Type == "email";
        }
    }
}
