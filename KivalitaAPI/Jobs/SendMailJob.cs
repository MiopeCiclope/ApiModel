﻿using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using KivalitaAPI;
using System.Web;
using System.Text;

[DisallowConcurrentExecution]
public class SendMailJob : IJob
{
    private readonly ILogger<SendMailJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;
    private MicrosoftTokenService graphService;
    private GraphServiceClient client;
    private int templateId = 4;
    private string userSignature = "";
    private int taskId = 0;

    public SendMailJob(ILogger<SendMailJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        this._serviceProvider = serviceProvider;
        semaphore = new Semaphore(1, 1);
    }

    public Task Execute(IJobExecutionContext context)
    {
        var userId = context.JobDetail.JobDataMap.GetInt("userId");
        this.taskId = context.JobDetail.JobDataMap.GetInt("taskId");

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

            this.graphService = scope.ServiceProvider.GetService<MicrosoftTokenService>();
            this.client = graphService.GetTokenClient(userId);
            this.userSignature = GetSignature(userId);

            var mailList = GetMailList(userId);

            if (!mailList.Any())
                _logger.LogInformation($"Nenhum e-mail na lista");
            else
                mailList.ForEach(mail =>
                {
                    var logMessage = graphService.SendMail(client, mail, userId) ? "Mail Sent" : "Faild Send Mail";
                    _logger.LogInformation($"{logMessage}: {mail.ToRecipients.First().EmailAddress.Address}");
                });
        }
        catch(Exception e)
        {
            _logger.LogError($"Erro ao enviar e-mail");
            _logger.LogError($"{e.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }

    private string GetSignature(int id)
    {
        var userService = scope.ServiceProvider.GetService<UserService>();
        return userService.Get(id)?.Signature ?? "";
    }

    private List<Message> GetMailList(int userId)
    {
        try
        {
            var leadService = scope.ServiceProvider.GetService<LeadsService>();
            var leadList = leadService.GetMailFromFlow(1);

            //var template = GetTemplate(templateId);
            //if (template == null) return null;

            return leadList
                    //.Where(lead => this.graphService.ShoulSendMail(this.client, lead.Email, userId))
                    .Select(lead => BuildEmail(lead, null)).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao buscar lista de e-mails");
            _logger.LogError($"{e.Message}");
            throw e;
        }
    }

    private Template GetTemplate(int id)
    {
        var templateService = scope.ServiceProvider.GetService<TemplateService>();
        return templateService.Get(id);
    }

    private Message BuildEmail(Leads lead, Template? template)
    {
        try
        {
            return new Message
            {
                Subject = "teste",
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = $"teste{GetTracker(lead)}{this.userSignature}"
                },
                ToRecipients = new List<Recipient>() {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "romulo.carvalho@kivalita.com.br"
                        }
                    } 
                }
            };
        }
        catch(Exception e)
        {
            _logger.LogError($"Erro ao criar e-mails");
            _logger.LogError($"{e.Message}");
            throw e;
        }
    }

    private string GetTracker(Leads lead)
    {
        var text = $"{this.taskId}-{lead.Id}";
        var encriptKey = HttpUtility.UrlEncode(AesCripty.EncryptString(Setting.MailTrackSecret, text), Encoding.UTF8);
        var url = $"<img src = \"http://api.kivalita.com.br.asp.hostazul.com.br/api/tracker/track?key={encriptKey}\" width=1 height=1 style=\"mso-hide:all; display:none; line-height: 0; font-size: 0; height: 0; padding: 0; visibility:hidden;\"/>";
        return url;
    }

    private string ReplaceVariables(string text, Leads lead)
    {
        try
        {
            string pattern = @"({{ (\w+)\.(\w+) }})";
            Regex variableRegex = new Regex(pattern);

            MatchCollection variables = variableRegex.Matches(text);

            if(variables.Any())
            {
                foreach (Match variable in variables)
                {
                    var entity = variable.Groups[2].Value;
                    var property = variable.Groups[3].Value;

                    if(entity == "lead")
                        text = text.Replace(variable.Value, lead.GetType().GetProperty(property).GetValue(lead, null)?.ToString()) ?? "";
                    if(entity == "company")
                        text = text.Replace(variable.Value, lead.Company.GetType().GetProperty(property).GetValue(lead.Company, null)?.ToString()) ?? "";
                }
            }
            return text;
        }
        catch(Exception e)
        {
            _logger.LogError($"Erro ao substituir variáveis");
            _logger.LogError($"{e.Message}");
            throw e;
        }
    }
}