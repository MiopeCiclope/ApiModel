using KivalitaAPI.Interfaces;
using KivalitaAPI.Repositories;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
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

        var thread = new Thread(() => { workerTask(taskId, userId); });
        thread.Start();
        return Task.CompletedTask;
    }

    public void workerTask(int taskId, int userId)
    {
        CancellationToken cancellationToken = new CancellationToken();

        var scope = this._serviceProvider.CreateScope();
        var flowRepository = scope.ServiceProvider.GetService<FlowRepository>();
        var flowTaskRepository = scope.ServiceProvider.GetService<FlowTaskRepository>();
        var flowTaskService = scope.ServiceProvider.GetService<FlowTaskService>();

        var flowTask = flowTaskRepository.Get(taskId);
        var flow = flowRepository.Get(flowTask.FlowAction.FlowId);

        if (flowTaskService.isJobAutomatic(flowTask.FlowAction) && flow.IsActive)
        {
            DateTimeOffset dateTime = new DateTimeOffset(DateTime.Now);
            var job = new JobScheduleDTO("SendMailJob", "0/2 * * * * ?", dateTime, flowTask.Id);
            job.userId = userId;
            _scheduler.ScheduleJob(cancellationToken, job);

            flowTask.Status = "finished";
            flowTaskService.Update(flowTask);
        }
    }
}