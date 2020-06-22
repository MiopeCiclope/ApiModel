using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class TokenRepository : Repository<Token, DbContext, SieveProcessor>
    {
        public TokenRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

