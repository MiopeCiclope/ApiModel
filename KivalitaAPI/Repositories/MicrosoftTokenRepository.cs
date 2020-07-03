
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class MicrosoftTokenRepository : Repository<MicrosoftToken, DbContext, SieveProcessor>
    {
        public MicrosoftTokenRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

