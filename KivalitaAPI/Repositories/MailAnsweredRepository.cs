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

        public int GetAmountAnsweredEmails(int flowid)
        {
            return context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                )
                .Select(mail => mail.TaskId)
                .Distinct()
                .Count();
        }

        public int GetAmountPositiveAnsweredEmails(int flowid)
        {
            return context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status == Enum.MailAnsweredStatusEnum.Positive
                )
                .Select(mail => mail.TaskId)
                .Distinct()
                .Count();
        }

        public int GetAmountNegativeAnsweredEmails(int flowid)
        {
            return context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status == Enum.MailAnsweredStatusEnum.Negative
                )
                .Select(mail => mail.TaskId)
                .Distinct()
                .Count();
        }

        public int GetAmountNotFoundAnsweredEmails(int flowid)
        {
            return context.Set<MailAnswered>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                    && mail.Status == Enum.MailAnsweredStatusEnum.NotFound
                )
                .Select(mail => mail.TaskId)
                .Distinct()
                .Count();
        }
    }
}
