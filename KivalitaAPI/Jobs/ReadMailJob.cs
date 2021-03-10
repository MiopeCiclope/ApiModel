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
public class ReadMailJob : IJob
{
    private readonly ILogger<ReadMailJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public ReadMailJob(ILogger<ReadMailJob> logger, IServiceProvider serviceProvider)
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

    public async virtual void  workerTask()
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"ReadMailJob - {DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var mailService = scope.ServiceProvider.GetService<MicrosoftTokenService>();
            var loggedUsers = mailService.GetAll()
                                        .Where(mail => mail.AccessToken != null)
                                        .ToList();

            foreach (var loggedUser in loggedUsers)
            {
                try
                {
                    await mailService.ReadMail(loggedUser.UserId);
                }
                catch (Exception e)
                {
                    _logger.LogError($"ReadMailJob - User: {loggedUser.UserId} - Id: {loggedUser.Id}");
                    _logger.LogError($"ReadMailJob - {e.Message}");
                }
            }

        }
        catch (Exception e)
        {
            _logger.LogError($"ReadMailJob - {e.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}