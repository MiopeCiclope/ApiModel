using KivalitaAPI.Repositories;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class SendMailJob : IJob
{
    private readonly ILogger<SendMailJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public SendMailJob(ILogger<SendMailJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        this._serviceProvider = serviceProvider;
        semaphore = new Semaphore(1, 1);
    }

    public Task Execute(IJobExecutionContext context)
    {
        var userId = context.JobDetail.JobDataMap.GetInt("userId");
        var thread = new Thread(() => { workerTask(userId); });
        thread.Start();
        return Task.CompletedTask;
    }

    public void workerTask(int userId)
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"{DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var graphService = scope.ServiceProvider.GetService<MicrosoftTokenService>();
            var leadService = scope.ServiceProvider.GetService<LeadsService>();

            var client = graphService.GetTokenClient(userId);

            var mailList = leadService.GetMailFromFlow(1);
            var mailsToSend = mailList
                .Where<string>(mail => graphService.ShoulSendMail(client, mail, userId))
                .Select(mail => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = mail
                    }
                });

            if (!mailsToSend.Any())
                _logger.LogInformation($"Nenhum e-mail na lista");
            else
            {
                var message = new Message
                {
                    Subject = "Meet for lunch?",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = "The new cafeteria is open."
                    },
                    ToRecipients = mailsToSend
                };

                var logMessage = graphService.SendMail(client, message, userId) ? "Mail Sent" : "Faild Send Mail";

                _logger.LogInformation($"{logMessage}");
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
}