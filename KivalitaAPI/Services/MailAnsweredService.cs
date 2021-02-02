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
        public MailAnsweredService(
            KivalitaApiContext context,
            MailAnsweredRepository baseRepository
        ) : base(context, baseRepository) { }

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

            return baseRepository.Add(mail);
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

            return baseRepository.Add(mail);
        }
    }
}
