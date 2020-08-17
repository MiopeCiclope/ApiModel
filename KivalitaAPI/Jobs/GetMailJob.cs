using KivalitaAPI.Models;
using KivalitaAPI.Queues;
using KivalitaAPI.Repositories;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class GetMailJob : IJob
{
    private readonly ILogger<BaseJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public GetMailJob(ILogger<BaseJob> logger, IServiceProvider serviceProvider)
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

    public void workerTask()
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"{DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var repo = scope.ServiceProvider.GetService<LeadsRepository>();
            var mailService = scope.ServiceProvider.GetService<GetEmailService>();

            var leadsWithoutEmail = repo.GetBy(lead => lead.CreatedAt.Date == DateTime.UtcNow.Date 
                                                        && lead.Email == null 
                                                        && !lead.DidGuessEmail);

            if (leadsWithoutEmail.Any())
            {
                foreach (Leads lead in leadsWithoutEmail)
                {
                   mailService.FromLeadAsync(lead).Wait();
                }
            } 
            else
            {
                _logger.LogInformation($"No Lead to process e-mail");
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Captura de e-mail on StartUp");
            _logger.LogInformation($"{e.Message}");
            _logger.LogInformation($"Fim do erro de e-mail on StartUp");
        }
        finally
        {
            semaphore.Release();
        }
    }
}