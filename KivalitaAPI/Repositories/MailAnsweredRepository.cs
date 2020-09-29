using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace KivalitaAPI.Repositories {
	public class MailAnsweredRepository : Repository<MailAnswered, DbContext, SieveProcessor> {
		public MailAnsweredRepository(DbContext context, SieveProcessor mailAnsweredProcessor) : base (context, mailAnsweredProcessor) { }
	}
}
