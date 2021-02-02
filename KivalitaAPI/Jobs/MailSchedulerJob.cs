using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using KivalitaAPI;
using System.Web;
using System.Text;
using KivalitaAPI.Repositories;
using KivalitaAPI.Enum;
using AutoMapper;

[DisallowConcurrentExecution]
public class MailSchedulerJob : IJob
{
    private readonly ILogger<SendMailJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;
    private MicrosoftTokenService graphService;
    private LogTaskService logTaskService;
    private GraphServiceClient client;
    private FlowTaskRepository taskRepository;
    private FlowTaskService flowTaskService;
    private int templateId = 4;
    private string userSignature = "";
    private FlowTaskDTORepository taskDtoRepository;
    private readonly IMapper _mapper;

    public MailSchedulerJob(ILogger<SendMailJob> logger, IMapper mapper, IServiceProvider serviceProvider)
    {
        _logger = logger;
        this._serviceProvider = serviceProvider;
        semaphore = new Semaphore(1, 1);
        _mapper = mapper;
    }

    public Task Execute(IJobExecutionContext context)
    {
         var thread = new Thread(() => { workerTask(); });
        thread.Start();
        return Task.CompletedTask;
    }

    public void workerTask()
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"Start Schedule e-mails - {DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            this.taskRepository = scope.ServiceProvider.GetService<FlowTaskRepository>();
            this.taskDtoRepository = scope.ServiceProvider.GetService<FlowTaskDTORepository>();
            this.graphService = scope.ServiceProvider.GetService<MicrosoftTokenService>();
            this.logTaskService = scope.ServiceProvider.GetService<LogTaskService>();
            this.flowTaskService = scope.ServiceProvider.GetService<FlowTaskService>();
            List<FlowTask> taskScheduledList = new List<FlowTask>();

            var taskList = taskRepository.GetSchedulableTask();
            foreach (var flowTask in taskList)
            {
                try
                {
                    var userId = flowTask.FlowAction.Flow.Owner ?? 0;
                    var taskId = flowTask.Id;

                    this.client = graphService.GetTokenClient(userId);
                    this.userSignature = GetSignature(userId);
                    this.templateId = (int)flowTask.FlowAction.TemplateId;

                    var mailList = GetMailList(userId, flowTask);

                    if (!mailList.Any())
                        _logger.LogInformation($"Nenhum e-mail na lista");
                    else
                        mailList.ForEach(mail =>
                        {
                            var logMessage = graphService.SendMail(client, mail, userId) ? "Mail Sent" : "Faild Send Mail";
                            if (logMessage == "Mail Sent")
                            {
                                this.logTaskService.RegisterLog(LogTaskEnum.EmailSent, flowTask.LeadId, flowTask.Id);
                                flowTask.Status = "finished";
                                taskScheduledList.Add(flowTask);
                            }
                            _logger.LogInformation($"{logMessage}: {mail.ToRecipients.First().EmailAddress.Address}");
                        });
                }
                catch (Exception e)
                {
                    _logger.LogError($"Erro ao montar tarefa");
                    _logger.LogError($"{e.Message}");
                }
            }

            if (taskScheduledList.Any())
            {
                var flowTaskDto = _mapper.Map<List<FlowTaskDatabaseDTO>>(taskScheduledList);
                taskDtoRepository.UpdateRange(flowTaskDto);
                foreach (var task in taskScheduledList)
                {
                    flowTaskService.scheduleNextTask(task);
                }
            }
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
        return userService.GetSignature(id);
    }

    private List<Message> GetMailList(int userId, FlowTask flowTask)
    {
        try
        {
            var leadService = scope.ServiceProvider.GetService<LeadsService>();
            var leadList = new List<Leads> { leadService.Get(flowTask.LeadId) };

            var template = GetTemplate(templateId);
            if (template == null) return null;

            return leadList
                    .Where(lead => !String.IsNullOrEmpty(lead.Email) && !this.graphService.DidReply(this.client, lead.Email))
                    .Select(lead => BuildEmail(lead, template, flowTask.Id)).ToList();
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

    private Message BuildEmail(Leads lead, Template template, int taskId)
    {
        try
        {
            return new Message
            {
                Subject = ReplaceVariables(template.Subject, lead),
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = $"{ReplaceVariables(template.Content, lead)}{GetTracker(lead, taskId)}{this.userSignature}"
                },
                ToRecipients = new List<Recipient>() {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = $"{lead.Email}"
                            //Address = $"romulo.carvalho@kivalita.com.br"
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

    private string GetTracker(Leads lead, int taskId)
    {
        var text = $"{taskId}-{lead.Id}";
        var encriptKey = HttpUtility.UrlEncode(AesCripty.EncryptString(Setting.MailTrackSecret, text), Encoding.UTF8);
        var url = $"<img src = \"{Setting.SelfUrl}tracker/track?key={encriptKey}\" width=1 height=1 style=\"mso-hide:all; display:none; line-height: 0; font-size: 0; height: 0; padding: 0; visibility:hidden;\"/>";
        return url;
    }

    private string ReplaceVariables(string text, Leads lead)
    {
        try
        {
            var templateTransformService = scope.ServiceProvider.GetService<TemplateTransformService>();

            return templateTransformService.TransformLead(text, lead);
        }
        catch(Exception e)
        {
            _logger.LogError($"Erro ao substituir variáveis");
            _logger.LogError($"{e.Message}");
            throw e;
        }
    }
}