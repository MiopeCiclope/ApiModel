
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class MailSignatureRepository : Repository<MailSignature, DbContext, SieveProcessor>
    {
        public MailSignatureRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

    