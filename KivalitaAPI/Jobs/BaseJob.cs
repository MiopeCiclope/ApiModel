using KivalitaAPI.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class BaseJob : IJob
{
    private readonly ILogger<BaseJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public BaseJob(ILogger<BaseJob> logger, IServiceProvider serviceProvider)
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

            var repo = scope.ServiceProvider.GetService<UserRepository>();
            var teste = repo.GetAll().First();
            _logger.LogInformation($"{teste.FirstName}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}