using System;
using System.Linq;
using System.Text.RegularExpressions;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.Graph;

namespace KivalitaAPI.Services
{

    public class MailAnsweredService : Service<MailAnswered, KivalitaApiContext, MailAnsweredRepository>
    {
        private LogTaskService logTaskService;
        private LeadsService leadService;

        public MailAnsweredService(
            KivalitaApiContext context,
            MailAnsweredRepository baseRepository,
            LogTaskService _logTaskService,
            LeadsService _leadService
        ) : base(context, baseRepository) {
            this.logTaskService = _logTaskService;
            this.leadService = _leadService;
        }

        public MailAnswered Save(Message message, int userId, int leadId, int taskId)
        {
            var messageFound = baseRepository.GetBy(
                mail => mail.MessageId == message.Id).FirstOrDefault();

            if (messageFound != null)
            {
                return messageFound;
            }

            var recipient = message.ToRecipients.First();

            MailAnswered mail = new MailAnswered
            {
                UserId = userId,
                LeadId = leadId,
                TaskId = taskId,
                MessageId = message.Id,
                Subject = message.Subject,
                BodyPreview = message.BodyPreview,
                BodyContent = message.Body.Content,
                Sender = message.Sender.EmailAddress.Address,
                Recipient = recipient.EmailAddress.Address,
                CreatedAt = DateTime.UtcNow
            };

            var answer = baseRepository.Add(mail);
            this.logTaskService.RegisterLog(LogTaskEnum.EmailAnswered, leadId, taskId, answer.Id);
            return answer;
        }

        public MailAnswered SaveQualified(Message message, int userId, int leadId, int taskId, MailAnsweredStatusEnum qualification)
        {
            var messageFound = baseRepository.GetBy(
                mail => mail.MessageId == message.Id).FirstOrDefault();

            if (messageFound != null)
            {
                return messageFound;
            }

            var recipient = message.ToRecipients.First();

            MailAnswered mail = new MailAnswered
            {
                UserId = userId,
                LeadId = leadId,
                TaskId = taskId,
                MessageId = message.Id,
                Subject = message.Subject,
                BodyPreview = message.BodyPreview,
                BodyContent = message.Body.Content,
                Sender = message.Sender.EmailAddress.Address,
                Recipient = recipient.EmailAddress.Address,
                CreatedAt = DateTime.UtcNow,
                Status = qualification
            };

            var answer = baseRepository.Add(mail);
            this.logTaskService.RegisterLog(LogTaskEnum.EmailAnswered, leadId, taskId, answer.Id);

            if (qualification == MailAnsweredStatusEnum.Negative) 
                removeLeadFromFlow(leadId);

            return null;
        }

        private void removeLeadFromFlow(int leadId)
        {
            var blackListLead = this.leadService.Get(leadId);
            blackListLead.Status = LeadStatusEnum.Blacklist;
            blackListLead.FlowId = null;
            this.leadService.Update(blackListLead);
        }

        public string GetLastReadMailId(int userId)
        {
            return baseRepository.GetLastReadMailId(userId);
        }
    }
}
