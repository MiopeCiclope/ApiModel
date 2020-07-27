using KivalitaAPI.Repositories;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class ReplyCheckJob : IJob
{
    private readonly ILogger<ReplyCheckJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public ReplyCheckJob(ILogger<ReplyCheckJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        this._serviceProvider = serviceProvider;
        semaphore = new Semaphore(1, 1);
    }

    public Task Execute(IJobExecutionContext context)
    {
        var thread = new Thread(workerTask);
        thread.Start();
        return Task.CompletedTask;
    }

    public virtual void workerTask()
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"{DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var service = scope.ServiceProvider.GetService<MicrosoftTokenService>();
            var userRepository = scope.ServiceProvider.GetService<UserRepository>();
            var leadRepository = scope.ServiceProvider.GetService<LeadsRepository>();

            var taskData = userRepository.GetTaskData();

            if(taskData.Any())
            {
                var usersWithTask = taskData.Select(t => t.UserId).Distinct();

                var LeadsToUpdateStatus = new List<int>();
                foreach (var user in usersWithTask)
                {
                    var client = service.GetTokenClient(user);
                    var emailToCheck = taskData.Where(t => t.UserId == user);

                    foreach (var task in emailToCheck)
                    {
                        var hasReply = service.DidReply(client, task.Email);
                        if (hasReply) LeadsToUpdateStatus.Add(task.LeadId);

                        _logger.LogInformation($"UserId: {user} - {task.Email}: DidReply {hasReply}");
                    }
                }

                if (LeadsToUpdateStatus.Any())
                {
                    leadRepository.UpdateStatusList(LeadsToUpdateStatus);
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
}