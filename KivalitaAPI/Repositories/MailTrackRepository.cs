
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class MailTrackRepository : Repository<MailTrack, DbContext, SieveProcessor>
    {
        public MailTrackRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public int GetAmountOpenedEmails(int flowid, int? templateId = null)
        {
            var query = context.Set<MailTrack>()
                .Where(
                    mail => mail.FlowTask.FlowAction.FlowId == flowid
                );

            if (templateId.HasValue)
                query = query.Where(mail => mail.FlowTask.FlowAction.TemplateId == templateId);

            return query.Select(mail => mail.TaskId).Distinct().Count();
        }
    }
}

