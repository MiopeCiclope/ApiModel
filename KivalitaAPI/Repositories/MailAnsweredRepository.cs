using System.Linq;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace KivalitaAPI.Repositories {
	public class MailAnsweredRepository : Repository<MailAnswered, DbContext, SieveProcessor> {

		public MailAnsweredRepository(DbContext context, SieveProcessor mailAnsweredProcessor) : base (context, mailAnsweredProcessor) { }

        public override MailAnswered Get(int id)
        {
            return context.Set<MailAnswered>()
                .Where(m => m.Id == id)
                .Include(m => m.FlowTask)
                .Include(m => m.Lead)
                .ThenInclude(l => l.Company)
                .SingleOrDefault();
        }

        public int GetAmountAnsweredEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status != null
                    && mail.Status != Enum.MailAnsweredStatusEnum.NotFound
                );

            if (templateId.HasValue)
                query = query.Where(mail => mail.FlowTask.FlowAction.TemplateId == templateId);
                            
            return query.Select(mail => mail.TaskId).Distinct().Count();
        }

        public int GetAmountPositiveAnsweredEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status != null
                    && mail.Status == Enum.MailAnsweredStatusEnum.Positive
                );

            if (templateId.HasValue)
                query = query.Where(mail => mail.FlowTask.FlowAction.TemplateId == templateId);

            return query.Select(mail => mail.TaskId).Distinct().Count();
        }

        public int GetAmountNegativeAnsweredEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status != null
                    && mail.Status == Enum.MailAnsweredStatusEnum.Negative
                );

            if (templateId.HasValue)
                query = query.Where(mail => mail.FlowTask.FlowAction.TemplateId == templateId);

            return query.Select(mail => mail.TaskId).Distinct().Count();
        }

        public int GetAmountNotFoundAnsweredEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status != null
                    && mail.Status == Enum.MailAnsweredStatusEnum.NotFound
                );

            if (templateId.HasValue)
                query = query.Where(mail => mail.FlowTask.FlowAction.TemplateId == templateId);

            return query.Select(mail => mail.TaskId).Distinct().Count();
        }

        public string GetLastReadMailId(int userId)
        {
            return context.Set<MailAnswered>()
                            .Where(mail => mail.UserId == userId)
                            .OrderByDescending(mail => mail.Id)
                            .Select(mail => mail.MessageId)
                            ?.First() ?? null;
        }
    }
}
