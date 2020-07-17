using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class TaskJob : IJob
{
    IServiceProvider _serviceProvider;
    private readonly ILogger<TaskJob> _logger;
    public readonly IJobScheduler _scheduler;

    public TaskJob(
        ILogger<TaskJob> logger,
        IJobScheduler scheduler,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _scheduler = scheduler;
        this._serviceProvider = serviceProvider;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var userId = context.JobDetail.JobDataMap.GetInt("userId");
        var taskId = context.JobDetail.JobDataMap.GetInt("taskId");

        var thread = new Thread(() => { workerTask(taskId); });
        thread.Start();
        return Task.CompletedTask;
    }

    public void workerTask(int taskId)
    {
        CancellationToken cancellationToken = new CancellationToken();

        var scope = this._serviceProvider.CreateScope();

        var flowTaskRepository = scope.ServiceProvider.GetService<FlowTaskRepository>();
        var flowActionRepository = scope.ServiceProvider.GetService<FlowActionRepository>();

        var flowTask = flowTaskRepository.Get(taskId);
        flowTask.Status = "finished";
        flowTaskRepository.Update(flowTask);

        var nextFlowTask = flowTaskRepository.context.Set<FlowTask>()
            .Where(f => f.Status == "pending" && f.LeadId == flowTask.LeadId)
            .Include(f => f.FlowAction)
            .FirstOrDefault();

        if (nextFlowTask != null)
        {
            var nextFlowAction = nextFlowTask.FlowAction;

            nextFlowTask.ScheduledTo = DateTime.Now.AddMinutes(nextFlowAction.afterDays);
            flowTaskRepository.Update(nextFlowTask);

            DateTimeOffset dateTimeOffset = new DateTimeOffset((DateTime)nextFlowTask.ScheduledTo);
            var job = new JobScheduleDTO("TaskJob", "0/2 * * * * ?", dateTimeOffset, nextFlowTask.Id);

            _scheduler.ScheduleJob(cancellationToken, job);
        }

    }
}