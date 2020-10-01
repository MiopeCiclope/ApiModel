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
                .SingleOrDefault();
        }
    }
}
